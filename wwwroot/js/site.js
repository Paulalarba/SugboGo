const clamp = (value, min = 0, max = 1) => Math.min(Math.max(value, min), max);
const easeInOutCubic = (value) => value < 0.5
    ? 4 * value * value * value
    : 1 - Math.pow(-2 * value + 2, 3) / 2;

const bounceEase = (value) => {
    const c1 = 1.70158;
    const c2 = c1 * 1.525;

    return value < 0.5
        ? (Math.pow(2 * value, 2) * ((c2 + 1) * 2 * value - c2)) / 2
        : (Math.pow(2 * value - 2, 2) * ((c2 + 1) * (value * 2 - 2) + c2) + 2) / 2;
};

const initMountainRange = async () => {
    const canvas = document.querySelector('[data-mountain-range]');

    if (!canvas || window.matchMedia('(prefers-reduced-motion: reduce)').matches) {
        return;
    }

    let THREE;

    try {
        THREE = await import('/lib/three/three.module.js');
    } catch {
        canvas.hidden = true;
        return;
    }

    const renderer = new THREE.WebGLRenderer({ canvas, alpha: true, antialias: true });
    const scene = new THREE.Scene();
    const camera = new THREE.OrthographicCamera(-10, 10, 5, -5, 0.1, 100);
    const range = new THREE.Group();
    const layerColors = ['#154E4E', '#1F6E6E', '#C4614A'];

    camera.position.z = 10;
    scene.add(range);

    const makeLayer = (index, yOffset, height, color) => {
        const shape = new THREE.Shape();
        const points = [
            [-11, -5],
            [-11, yOffset],
            [-8.5, yOffset + height * 0.55],
            [-6.7, yOffset + height * 0.22],
            [-4.8, yOffset + height * 0.8],
            [-2.2, yOffset + height * 0.28],
            [0.4, yOffset + height],
            [2.5, yOffset + height * 0.35],
            [5.2, yOffset + height * 0.72],
            [7.2, yOffset + height * 0.3],
            [10.5, yOffset + height * 0.62],
            [11, yOffset],
            [11, -5]
        ];

        points.forEach(([x, y], pointIndex) => {
            if (pointIndex === 0) {
                shape.moveTo(x, y);
            } else {
                shape.lineTo(x, y);
            }
        });

        const geometry = new THREE.ShapeGeometry(shape);
        const material = new THREE.MeshBasicMaterial({
            color,
            transparent: true,
            opacity: 0.16 + index * 0.08,
            depthWrite: false
        });
        const mesh = new THREE.Mesh(geometry, material);

        mesh.position.z = -index * 0.1;
        mesh.position.y = -index * 0.42;
        range.add(mesh);
    };

    layerColors.forEach((color, index) => makeLayer(index, -2.9 + index * 0.32, 1.2 + index * 0.28, color));

    const resize = () => {
        const rect = canvas.getBoundingClientRect();
        const width = Math.max(rect.width, 1);
        const height = Math.max(rect.height, 1);

        renderer.setPixelRatio(Math.min(window.devicePixelRatio, 2));
        renderer.setSize(width, height, false);
        camera.left = -10;
        camera.right = 10;
        camera.top = 10 * (height / width);
        camera.bottom = -10 * (height / width);
        camera.updateProjectionMatrix();
    };

    const update = () => {
        const progress = clamp(window.scrollY / 300);
        const blurProgress = progress < 0.5 ? progress * 2 : (1 - progress) * 2;
        const slowScroll = window.scrollY * 0.3;

        canvas.style.opacity = String(0.2 + progress * 0.6);
        canvas.style.filter = `sepia(${1 - progress}) blur(${2 * blurProgress}px)`;
        canvas.style.transform = `translate3d(0, ${-slowScroll}px, 0)`;
        range.position.x = (progress - 0.5) * 0.7;
        range.position.y = progress * 0.18;

        renderer.render(scene, camera);
        requestAnimationFrame(update);
    };

    resize();
    window.addEventListener('resize', resize);
    update();
};

const initDestinationCinema = () => {
    const section = document.querySelector('[data-destination-cinema]');
    const rail = document.querySelector('[data-card-rail]');

    if (!section || !rail) {
        return;
    }

    const cards = [...rail.querySelectorAll('[data-destination-card]')];
    const imageFrames = [...rail.querySelectorAll('[data-cinematic-image]')];
    let lastScrollLeft = rail.scrollLeft;
    let snapTimer;

    const getProgress = () => {
        const maxScroll = Math.max(rail.scrollWidth - rail.clientWidth, 1);
        return clamp(rail.scrollLeft / maxScroll);
    };

    const snapToNearestCard = () => {
        const railCenter = rail.scrollLeft + rail.clientWidth / 2;
        const nearest = cards.reduce((current, card) => {
            const cardCenter = card.offsetLeft + card.offsetWidth / 2;
            const distance = Math.abs(cardCenter - railCenter);

            return distance < current.distance ? { distance, cardCenter } : current;
        }, { distance: Infinity, cardCenter: cards[0]?.offsetWidth / 2 || 0 });

        rail.scrollTo({
            left: nearest.cardCenter - rail.clientWidth / 2,
            behavior: 'smooth'
        });
    };

    const revealImages = () => {
        const viewportHeight = window.innerHeight;

        imageFrames.forEach((frame) => {
            const rect = frame.getBoundingClientRect();
            const triggerStart = viewportHeight + 100;
            const triggerEnd = viewportHeight - 200;
            const progress = clamp((triggerStart - rect.top) / Math.max(triggerStart - triggerEnd, 1));
            const eased = easeInOutCubic(progress);

            if (progress > 0.05) {
                frame.classList.add('is-revealed');
            }

            frame.style.setProperty('--sg-image-scale', String(1 + eased * 0.25));
            frame.style.setProperty('--sg-image-pan-x', `${eased * 5}%`);
            frame.style.setProperty('--sg-image-pan-y', `${50 + eased * 3}%`);
        });
    };

    const updateCards = () => {
        const direction = Math.sign(rail.scrollLeft - lastScrollLeft);
        const progress = getProgress();
        const easedProgress = clamp(bounceEase(progress));
        const cameraDegrees = -20 + easedProgress * 40;

        section.style.setProperty('--sg-card-pan', `${50 + cameraDegrees * 0.9}%`);

        cards.forEach((card) => {
            const railCenter = rail.scrollLeft + rail.clientWidth / 2;
            const cardCenter = card.offsetLeft + card.offsetWidth / 2;
            const distance = Math.abs(cardCenter - railCenter);
            const normalized = clamp(distance / Math.max(card.offsetWidth * 1.2, 1));
            const active = 1 - normalized;
            const tilt = (direction || Math.sign(cardCenter - railCenter) || 1) * (3 + normalized * 2);

            card.style.setProperty('--sg-card-opacity', String(0.4 + active * 0.6));
            card.style.setProperty('--sg-card-tilt', `${tilt}deg`);
            card.style.setProperty('--sg-card-skew', `${(direction || 1) * 1.5 * active}deg`);
            card.style.boxShadow = `0 ${10 + active * 10}px ${20 + active * 20}px rgba(26,26,24,${0.12 + active * 0.08})`;
        });

        lastScrollLeft = rail.scrollLeft;
        revealImages();
    };

    rail.addEventListener('scroll', () => {
        updateCards();
        window.clearTimeout(snapTimer);
        snapTimer = window.setTimeout(snapToNearestCard, 300);
    }, { passive: true });

    rail.addEventListener('wheel', (event) => {
        const canScrollHorizontally = rail.scrollWidth > rail.clientWidth;
        const sectionRect = section.getBoundingClientRect();
        const isInsideSection = sectionRect.top < window.innerHeight * 0.72 && sectionRect.bottom > window.innerHeight * 0.28;

        if (!canScrollHorizontally || !isInsideSection || Math.abs(event.deltaX) > Math.abs(event.deltaY)) {
            return;
        }

        const atStart = rail.scrollLeft <= 0 && event.deltaY < 0;
        const atEnd = rail.scrollLeft >= rail.scrollWidth - rail.clientWidth - 1 && event.deltaY > 0;

        if (atStart || atEnd) {
            return;
        }

        event.preventDefault();
        rail.scrollLeft += event.deltaY;
    }, { passive: false });

    window.addEventListener('scroll', revealImages, { passive: true });
    window.addEventListener('resize', updateCards);
    updateCards();
};

const initLandingPage = () => {
    const revealItems = [...document.querySelectorAll('.lp-reveal')];
    const counters = [...document.querySelectorAll('[data-count]')];
    const timeline = document.querySelector('[data-timeline]');
    const accordions = [...document.querySelectorAll('[data-accordion] .lp-faq-item')];

    if (!revealItems.length && !counters.length && !accordions.length) {
        return;
    }

    const prefersReducedMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;
    const countUp = (element) => {
        if (element.dataset.counted === 'true') {
            return;
        }

        const target = Number(element.dataset.count || '0');
        const duration = prefersReducedMotion ? 0 : 900;
        const start = performance.now();

        element.dataset.counted = 'true';

        const tick = (now) => {
            const progress = duration === 0 ? 1 : clamp((now - start) / duration);
            const eased = 1 - Math.pow(1 - progress, 4);
            const value = Math.round(target * eased);

            element.textContent = value.toLocaleString();

            if (progress < 1) {
                requestAnimationFrame(tick);
            }
        };

        requestAnimationFrame(tick);
    };

    const revealObserver = new IntersectionObserver((entries) => {
        entries.forEach((entry) => {
            if (!entry.isIntersecting) {
                return;
            }

            entry.target.classList.add('is-visible');
            revealObserver.unobserve(entry.target);
        });
    }, {
        threshold: 0.16,
        rootMargin: '0px 0px -8% 0px'
    });

    revealItems.forEach((item, index) => {
        item.style.transitionDelay = `${Math.min(index % 5, 4) * 90}ms`;
        revealObserver.observe(item);
    });

    const counterObserver = new IntersectionObserver((entries) => {
        entries.forEach((entry) => {
            if (!entry.isIntersecting) {
                return;
            }

            countUp(entry.target);
            counterObserver.unobserve(entry.target);
        });
    }, { threshold: 0.35 });

    counters.forEach((counter) => counterObserver.observe(counter));

    if (timeline) {
        const updateTimeline = () => {
            const rect = timeline.getBoundingClientRect();
            const viewport = window.innerHeight;
            const progress = clamp((viewport * 0.82 - rect.top) / Math.max(rect.height, 1));

            timeline.style.setProperty('--timeline-progress', `${progress * 100}%`);
        };

        window.addEventListener('scroll', updateTimeline, { passive: true });
        window.addEventListener('resize', updateTimeline);
        updateTimeline();
    }

    accordions.forEach((item, index) => {
        const button = item.querySelector('button');

        if (!button) {
            return;
        }

        if (index === 0) {
            item.classList.add('is-open');
            button.setAttribute('aria-expanded', 'true');
        }

        button.addEventListener('click', () => {
            const willOpen = !item.classList.contains('is-open');

            accordions.forEach((other) => {
                other.classList.remove('is-open');
                other.querySelector('button')?.setAttribute('aria-expanded', 'false');
            });

            if (willOpen) {
                item.classList.add('is-open');
                button.setAttribute('aria-expanded', 'true');
            }
        });
    });
};

const initBookingFlow = () => {
    const wizard = document.querySelector('[data-booking-wizard]');

    if (!wizard) {
        return;
    }

    const panels = [...wizard.querySelectorAll('[data-booking-step]')];
    const indicators = [...wizard.querySelectorAll('[data-step-indicator]')];
    const basePrice = Number(wizard.dataset.basePrice || '0');
    const destinationId = wizard.dataset.destinationId || '';
    const bookingType = wizard.dataset.bookingType || 'UserSelected';
    const destination = wizard.dataset.destination || '';
    const location = wizard.dataset.location || 'Cebu, Philippines';
    const imageUrl = wizard.dataset.imageUrl || wizard.querySelector('.booking-hero img')?.src || '';
    const antiForgeryToken = wizard.querySelector('input[name="__RequestVerificationToken"]')?.value || '';

    // Elements
    const totalDisplay = wizard.querySelector('[data-total-display]');
    const addonsDisplay = wizard.querySelector('[data-addons-display]');
    const addonsRow = wizard.querySelector('[data-add-ons-row]');
    const taxesDisplay = wizard.querySelector('[data-taxes-display]');
    const reviewSummary = wizard.querySelector('[data-review-summary]');
    const paymentSim = wizard.querySelector('[data-payment-sim]');
    const confIdDisplay = wizard.querySelector('[data-conf-id]');

    const state = {
        destinationId,
        bookingType,
        destinationName: destination,
        imageUrl,
        location,
        travelDate: '',
        travelerType: 'Solo',
        travelerCount: 1,
        selectedActivities: [],
        selectedAccommodation: '',
        selectedTransportation: '',
        travelerNotes: '',
        basePrice: basePrice,
        addonsPrice: 0,
        taxesAndFees: 0,
        totalPrice: basePrice
    };

    const updatePricing = () => {
        let addons = 0;

        // Activities
        wizard.querySelectorAll('input[name="activities"]:checked').forEach((cb) => {
            addons += Number(cb.dataset.price || '0');
        });

        // Accommodation
        const selectedAcc = wizard.querySelector('input[name="accommodation"]:checked');
        addons += Number(selectedAcc?.dataset.price || '0');

        // Transportation
        const selectedTrans = wizard.querySelector('input[name="transportation"]:checked');
        addons += Number(selectedTrans?.dataset.price || '0');

        const taxes = Math.round((basePrice + addons) * 0.12);
        const total = basePrice + addons + taxes;

        state.addonsPrice = addons;
        state.addOnsPrice = addons;
        state.taxesAndFees = taxes;
        state.totalPrice = total;

        if (totalDisplay) totalDisplay.textContent = total.toLocaleString();
        if (addonsDisplay) addonsDisplay.textContent = addons.toLocaleString();
        if (taxesDisplay) taxesDisplay.textContent = taxes.toLocaleString();
        if (addonsRow) addonsRow.hidden = addons === 0;
    };

    const setStep = (stepName) => {
        panels.forEach((p) => {
            const isActive = p.dataset.bookingStep === stepName;
            p.classList.toggle('is-active', isActive);
        });

        indicators.forEach((ind) => {
            ind.classList.toggle('is-active', ind.dataset.stepIndicator === stepName);
        });

        if (stepName === 'review') {
            renderReview();
        }

        wizard.scrollIntoView({ behavior: 'smooth', block: 'start' });
    };

    const renderReview = () => {
        if (!reviewSummary) return;

        state.travelDate = wizard.querySelector('input[name="travelDate"]')?.value || '';
        const travelerSelect = wizard.querySelector('select[name="travelerType"]');
        state.travelerType = travelerSelect?.value || wizard.querySelector('input[name="travelerType"]:checked')?.value || 'Solo';
        state.travelerCount = Number(travelerSelect?.selectedOptions?.[0]?.dataset.count || state.travelerCount || '1');
        state.selectedActivities = [...wizard.querySelectorAll('input[name="activities"]:checked')].map(cb => cb.value);
        state.selectedAccommodation = wizard.querySelector('input[name="accommodation"]:checked')?.value || '';
        state.selectedTransportation = wizard.querySelector('input[name="transportation"]:checked')?.value || '';
        state.travelerNotes = wizard.querySelector('textarea[name="travelerNotes"]')?.value || '';

        const row = (label, value) => `<div class="review-item"><strong>${escapeHtml(label)}</strong><span>${escapeHtml(value)}</span></div>`;
        reviewSummary.innerHTML = `
            <div class="review-grid">
                ${row('Destination', state.destinationName)}
                ${row('Travel date', state.travelDate)}
                ${row('Travelers', `${state.travelerType} (${state.travelerCount})`)}
                ${row('Stay', state.selectedAccommodation)}
                ${row('Transport', state.selectedTransportation)}
                ${row('Activities', state.selectedActivities.length > 0 ? state.selectedActivities.join(', ') : 'None selected')}
                ${row('Notes', state.travelerNotes || 'No specific requests.')}
                <div class="review-total"><strong>Final price: PHP ${state.totalPrice.toLocaleString()}</strong></div>
            </div>
        `;
    };

    const handlePayment = (method) => {
        if (paymentSim) paymentSim.hidden = false;

        state.paymentMethod = method;
        renderReview();

        // Send to server
        fetch('/Booking/ConfirmBooking', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': antiForgeryToken
            },
            body: JSON.stringify(state)
        })
            .then(res => res.json())
            .then(data => {
                if (data.success) {
                    if (confIdDisplay) confIdDisplay.textContent = data.bookingId.substring(0, 8).toUpperCase();
                    renderQr(data.qrCode || data.bookingId);
                    setTimeout(() => setStep('success'), 1500);
                }
            })
            .catch(() => {
                if (paymentSim) {
                    paymentSim.innerHTML = '<p>Payment could not be completed. Please try again.</p>';
                }
            });
    };

    const escapeHtml = (value) => String(value)
        .replaceAll('&', '&amp;')
        .replaceAll('<', '&lt;')
        .replaceAll('>', '&gt;')
        .replaceAll('"', '&quot;')
        .replaceAll("'", '&#039;');

    const renderQr = (code) => {
        const target = wizard.querySelector('[data-qr-code-placeholder]');

        if (!target) {
            return;
        }

        const bits = String(code || 'SUGBOGO').padEnd(25, '0').slice(0, 25);
        target.innerHTML = bits.split('').map((char, index) => {
            const filled = ((char.charCodeAt(0) + index) % 3) !== 0;
            return `<i class="${filled ? 'is-filled' : ''}"></i>`;
        }).join('');
    };

    // Events
    wizard.querySelectorAll('[data-goto-step]').forEach(btn => {
        btn.addEventListener('click', () => setStep(btn.dataset.gotoStep));
    });

    wizard.querySelectorAll('input, select').forEach(input => {
        input.addEventListener('change', updatePricing);
    });

    wizard.querySelectorAll('[data-pay-method]').forEach(btn => {
        btn.addEventListener('click', () => handlePayment(btn.dataset.payMethod));
    });

    updatePricing();
};

const initAccountFlow = () => {
    const emailForm = document.querySelector('[data-auth-email-form]');

    emailForm?.addEventListener('submit', () => {
        const button = emailForm.querySelector('.auth-primary');

        if (!button) {
            return;
        }

        button.classList.add('is-loading');
        button.textContent = 'Checking account...';
    });
};

const initDashboard = () => {
    const dashboard = document.querySelector('[data-dashboard]');

    if (!dashboard) {
        return;
    }

    const vibeTags = [...dashboard.querySelectorAll('[data-vibe-tag]')];
    const surpriseButton = dashboard.querySelector('[data-surprise-button]');
    const surprisePanel = dashboard.querySelector('[data-surprise-panel]');
    const surpriseTitle = dashboard.querySelector('[data-surprise-title]');
    const surpriseReason = dashboard.querySelector('[data-surprise-reason]');
    const gems = [...dashboard.querySelectorAll('[data-surprise-gem]')];
    const feedForm = dashboard.querySelector('[data-feed-form]');
    const feedList = dashboard.querySelector('[data-feed-list]');
    const photoInput = dashboard.querySelector('[data-feed-photo]');
    const photoPreview = dashboard.querySelector('[data-feed-preview]');
    const focusComposer = dashboard.querySelector('[data-focus-composer]');
    const currentInitial = dashboard.querySelector('.feed-profile-chip')?.textContent?.trim() || 'T';
    const currentName = dashboard.querySelector('.feed-profile-card h2')?.textContent?.trim() || 'Traveler';
    const antiForgeryToken = dashboard.querySelector('input[name="__RequestVerificationToken"]')?.value || '';
    const savedList = dashboard.querySelector('[data-saved-list]');
    const noSavedGemsMessage = dashboard.querySelector('[data-no-saved-gems]');
    const escapeHtml = (value) => String(value)
        .replaceAll('&', '&amp;')
        .replaceAll('<', '&lt;')
        .replaceAll('>', '&gt;')
        .replaceAll('"', '&quot;')
        .replaceAll("'", '&#039;');

    const bindSavedGem = (li) => {
        const removeBtn = li.querySelector('[data-remove-gem]');

        removeBtn?.addEventListener('click', () => {
            const removeUrl = removeBtn.dataset.removeUrl;

            if (removeUrl) {
                fetch(removeUrl, {
                    method: 'POST',
                    headers: {
                        'RequestVerificationToken': antiForgeryToken
                    }
                }).then((response) => {
                    if (response.ok) {
                        li.remove();

                        if (savedList && savedList.children.length === 0) {
                            savedList.remove();
                            const p = document.createElement('p');
                            p.dataset.noSavedGems = '';
                            p.textContent = 'Saved gems will appear here once save actions are persisted.';
                            dashboard.querySelector('#gem-vault-panel')?.appendChild(p);
                        }
                    }
                });
            }
        });
    };

    dashboard.querySelectorAll('[data-saved-list] li').forEach(bindSavedGem);

    dashboard.querySelectorAll('[data-save-gem]').forEach((btn) => {
        btn.addEventListener('click', () => {
            const saveUrl = btn.dataset.saveUrl;

            if (saveUrl) {
                fetch(saveUrl, {
                    method: 'POST',
                    headers: {
                        'RequestVerificationToken': antiForgeryToken
                    }
                })
                    .then((response) => response.ok ? response.json() : null)
                    .then((data) => {
                        if (!data) {
                            return;
                        }

                        btn.textContent = 'Saved';
                        btn.disabled = true;

                        let list = dashboard.querySelector('[data-saved-list]');

                        if (!list) {
                            dashboard.querySelector('[data-no-saved-gems]')?.remove();
                            list = document.createElement('ul');
                            list.className = 'feed-saved-list';
                            list.dataset.savedList = '';
                            dashboard.querySelector('#gem-vault-panel')?.appendChild(list);
                        }

                        const li = document.createElement('li');
                        li.dataset.savedGemId = data.id;
                        li.innerHTML = `
                            <div>
                                <strong>${escapeHtml(data.title)}</strong>
                                <span>Just saved from recommendations.</span>
                            </div>
                            <button type="button" data-remove-gem data-remove-url="/Dashboard/RemoveGem?gemId=${data.id}" aria-label="Remove gem">&times;</button>`;

                        list.prepend(li);
                        bindSavedGem(li);
                    });
            }
        });
    });

    vibeTags.forEach((tag) => {
        tag.addEventListener('click', () => {
            tag.classList.toggle('is-active');
        });
    });

    surpriseButton?.addEventListener('click', () => {
        if (!gems.length || !surprisePanel || !surpriseTitle || !surpriseReason) {
            return;
        }

        const index = Math.floor(Math.random() * gems.length);
        const gem = gems[index];

        gems.forEach((item) => item.classList.remove('is-featured'));
        gem.classList.add('is-featured');
        surpriseTitle.textContent = gem.dataset.title || 'A hidden Cebu gem';
        surpriseReason.textContent = gem.dataset.reason || 'Matched to your travel profile.';
        surprisePanel.hidden = false;
        surprisePanel.scrollIntoView({ behavior: 'smooth', block: 'center' });
    });

    const bindFeedPost = (post) => {
        const likeButton = post.querySelector('[data-like-button]');
        const commentToggle = post.querySelector('[data-comment-toggle]');
        const shareButton = post.querySelector('[data-share-button]');
        const commentForm = post.querySelector('[data-comment-form]');
        const commentInput = commentForm?.querySelector('input');
        const comments = post.querySelector('[data-comments]');
        const likeCount = post.querySelector('[data-like-count]');
        const commentCount = post.querySelector('[data-comment-count]');

        likeButton?.addEventListener('click', () => {
            const likeUrl = likeButton.dataset.likeUrl;

            if (likeUrl) {
                fetch(likeUrl, {
                    method: 'POST',
                    headers: {
                        'RequestVerificationToken': antiForgeryToken
                    }
                })
                    .then((response) => response.ok ? response.json() : null)
                    .then((data) => {
                        if (!data || !likeCount) {
                            return;
                        }

                        likeButton.classList.add('is-liked');
                        likeCount.textContent = String(data.likes);
                    })
                    .catch(() => { });
                return;
            }

            const isLiked = likeButton.classList.toggle('is-liked');
            const currentLikes = Number(likeCount?.textContent || '0');

            if (likeCount) {
                likeCount.textContent = String(Math.max(0, currentLikes + (isLiked ? 1 : -1)));
            }
        });

        commentToggle?.addEventListener('click', () => {
            commentInput?.focus();
        });

        shareButton?.addEventListener('click', async () => {
            const destination = post.querySelector('.feed-post__title-row h2')?.textContent?.trim() || 'Cebu destination';
            const text = `Check out ${destination} on SugboGo.`;

            if (navigator.share) {
                await navigator.share({ title: destination, text }).catch(() => { });
                return;
            }

            await navigator.clipboard?.writeText(text).catch(() => { });
            shareButton.textContent = 'Copied';
            window.setTimeout(() => {
                shareButton.textContent = 'Share';
            }, 1200);
        });

        commentForm?.addEventListener('submit', (event) => {
            event.preventDefault();

            if (!commentInput || !comments || !commentInput.value.trim()) {
                return;
            }

            const commentUrl = commentForm.dataset.commentUrl;
            const text = commentInput.value.trim();

            if (commentUrl) {
                const formData = new FormData();
                formData.append('text', text);

                fetch(commentUrl, {
                    method: 'POST',
                    headers: {
                        'RequestVerificationToken': antiForgeryToken
                    },
                    body: formData
                })
                    .then((response) => response.ok ? response.json() : null)
                    .then((data) => {
                        if (!data) {
                            return;
                        }

                        const comment = document.createElement('div');
                        comment.className = 'feed-comment';
                        comment.innerHTML = `<strong>${escapeHtml(data.authorName)}</strong> ${escapeHtml(data.text)} <small>${escapeHtml(data.timestamp)}</small>`;
                        comments.prepend(comment);

                        if (commentCount) {
                            commentCount.textContent = String(data.commentCount);
                        }

                        commentInput.value = '';
                    })
                    .catch(() => { });
                return;
            }

            const comment = document.createElement('div');
            comment.className = 'feed-comment';
            comment.innerHTML = `<strong>${escapeHtml(currentName)}</strong> ${escapeHtml(text)}`;
            comments.prepend(comment);

            if (commentCount) {
                commentCount.textContent = String(Number(commentCount.textContent || '0') + 1);
            }

            commentInput.value = '';
        });
    };

    dashboard.querySelectorAll('[data-feed-post]').forEach(bindFeedPost);

    focusComposer?.addEventListener('click', () => {
        feedForm?.querySelector('input[name="destination"]')?.focus();
    });

    photoInput?.addEventListener('change', () => {
        const file = photoInput.files?.[0];

        if (!file || !photoPreview) {
            return;
        }

        photoPreview.src = URL.createObjectURL(file);
        photoPreview.hidden = false;
    });

    feedForm?.addEventListener('submit', (event) => {
        if (feedForm.dataset.serverForm !== undefined) {
            return;
        }

        event.preventDefault();

        if (!feedList) {
            return;
        }

        const data = new FormData(feedForm);
        const destination = String(data.get('destination') || '').trim();
        const location = String(data.get('location') || '').trim();
        const description = String(data.get('description') || '').trim();
        const caption = String(data.get('caption') || '').trim();
        const tag = String(data.get('tag') || 'Cebu').trim();
        const imageUrl = photoPreview && !photoPreview.hidden
            ? photoPreview.src
            : 'https://images.unsplash.com/photo-1507525428034-b723cf961d3e?auto=format&fit=crop&w=1200&q=80';

        if (!destination || !location || !description) {
            return;
        }

        const post = document.createElement('article');
        post.className = 'feed-post';
        post.dataset.feedPost = '';
        post.innerHTML = `
            <header class="feed-post__header">
                <div class="feed-avatar">${escapeHtml(currentInitial)}</div>
                <div>
                    <strong>${escapeHtml(currentName)}</strong>
                    <span>SugboGo client · Just now</span>
                </div>
            </header>
            <div class="feed-post__body">
                <div class="feed-post__title-row">
                    <div>
                        <h2>${escapeHtml(destination)}</h2>
                        <p>${escapeHtml(location)}</p>
                    </div>
                    <span>New AI signal</span>
                </div>
                <p>${escapeHtml(description)}</p>
                <p class="feed-caption">${escapeHtml(caption || 'Fresh Cebu travel note.')}</p>
                <div class="feed-tags">
                    <span>#${escapeHtml(tag)}</span>
                    <span>#Cebu</span>
                </div>
            </div>
            <img class="feed-post__image" src="${escapeHtml(imageUrl)}" alt="${escapeHtml(destination)} in ${escapeHtml(location)}" />
            <div class="feed-ai-note">
                <strong>Recommended for others</strong>
                <span>This post will train recommendations through its tags, likes, comments, and destination location.</span>
            </div>
            <footer class="feed-engagement">
                <div class="feed-counts">
                    <span><b data-like-count>0</b> likes</span>
                    <span><b data-comment-count>0</b> comments</span>
                </div>
                <div class="feed-actions">
                    <button type="button" data-like-button>Like</button>
                    <button type="button" data-comment-toggle>Comment</button>
                    <button type="button" data-share-button>Share</button>
                </div>
                <form class="feed-comment-form" data-comment-form>
                    <input type="text" placeholder="Write a comment..." aria-label="Write a comment" required />
                    <button type="submit">Send</button>
                </form>
                <div class="feed-comments" data-comments></div>
            </footer>`;

        feedList.prepend(post);
        bindFeedPost(post);
        feedForm.reset();

        if (photoPreview) {
            photoPreview.hidden = true;
            photoPreview.removeAttribute('src');
        }

        post.scrollIntoView({ behavior: 'smooth', block: 'start' });
    });
};

const initAdminDashboard = () => {
    const admin = document.querySelector('[data-admin-dashboard]');

    if (!admin) {
        return;
    }

    const cards = [...admin.querySelectorAll('.admin-kanban-card')];
    const columns = [...admin.querySelectorAll('.admin-kanban__column')];
    const approvalButtons = [...admin.querySelectorAll('[data-admin-approve]')];

    cards.forEach((card) => {
        card.addEventListener('dragstart', () => {
            card.classList.add('is-dragging');
        });

        card.addEventListener('dragend', () => {
            card.classList.remove('is-dragging');
            columns.forEach((column) => {
                const count = column.querySelectorAll('.admin-kanban-card').length;
                const badge = column.querySelector('h3 span');

                if (badge) {
                    badge.textContent = String(count);
                }
            });
        });
    });

    columns.forEach((column) => {
        column.addEventListener('dragover', (event) => {
            event.preventDefault();
            const draggingCard = admin.querySelector('.admin-kanban-card.is-dragging');

            if (draggingCard) {
                column.appendChild(draggingCard);
            }
        });
    });

    approvalButtons.forEach((button) => {
        button.addEventListener('click', () => {
            const row = button.closest('article');
            const status = row?.querySelector('small');

            button.textContent = 'Approved';
            button.disabled = true;

            if (status) {
                status.textContent = 'Approved for itinerary mapping';
            }
        });
    });
};

const initSessionGuard = () => {
    const isAuthenticated = document.body.dataset.authenticated === 'true';

    if (!isAuthenticated) {
        sessionStorage.removeItem('sg_session_active');
        return;
    }

    // Mark this tab's session as active so authenticated pages load normally.
    // Previously this flag was set AFTER a logout check, which meant every
    // fresh sign-in was immediately logged out before the dashboard loaded.
    sessionStorage.setItem('sg_session_active', 'true');
};

document.addEventListener('DOMContentLoaded', () => {
    document.documentElement.classList.add('sg-motion-ready');
    initSessionGuard();
    initMountainRange();
    initDestinationCinema();
    initLandingPage();
    initBookingFlow();
    initAccountFlow();
    initDashboard();
    initAdminDashboard();
});

/* Booking Modal Logic */
document.addEventListener('click', (e) => {
    if (e.target.closest('.dest-card')) {
        e.target.closest('.booking-modal')?.setAttribute('hidden', '');
        return;
    }

    if (e.target.closest('[data-modal-open]')) {
        e.preventDefault();
        const id = e.target.closest('[data-modal-open]').getAttribute('href');
        document.querySelector(id)?.removeAttribute('hidden');
    }
    if (e.target.closest('[data-modal-close]')) {
        e.target.closest('.booking-modal').setAttribute('hidden', '');
    }
    if (e.target.classList.contains('booking-modal')) {
        e.target.setAttribute('hidden', '');
    }
});
