(function () {
  'use strict';

  const TOKEN_KEY = 'moviereview_token';
  const API_BASE = '';

  function getToken() {
    return localStorage.getItem(TOKEN_KEY);
  }

  function setToken(token) {
    if (token) localStorage.setItem(TOKEN_KEY, token);
    else localStorage.removeItem(TOKEN_KEY);
  }

  function isLoggedIn() {
    return !!getToken();
  }

  /** Get current user id from JWT (sub claim), or null. */
  function getCurrentUserId() {
    const token = getToken();
    if (!token) return null;
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const sub = payload.sub;
      return sub ? parseInt(sub, 10) : null;
    } catch (_) {
      return null;
    }
  }

  async function api(path, options = {}) {
    const url = `${API_BASE}${path}`;
    const headers = {
      'Content-Type': 'application/json',
      ...options.headers,
    };
    const token = getToken();
    if (token) headers['Authorization'] = `Bearer ${token}`;

    const res = await fetch(url, { ...options, headers });
    const text = await res.text();
    let data = null;
    try {
      data = text ? JSON.parse(text) : null;
    } catch (_) {}

    if (!res.ok) {
      const err = new Error(data?.message || res.statusText || 'Request failed');
      err.status = res.status;
      err.data = data;
      throw err;
    }
    return data;
  }

  // --- Routing ---
  function getHashRoute() {
    const hash = (window.location.hash || '#/login').slice(1);
    const [path, id] = hash.split('/').filter(Boolean);
    return { path: path || 'login', id: id || null };
  }

  function setHash(path, id) {
    const part = id ? `${path}/${id}` : path;
    window.location.hash = part ? '#' + part : '#/movies';
  }

  function showPage(pageId) {
    document.querySelectorAll('[data-page]').forEach((el) => {
      el.removeAttribute('data-active');
    });
    const page = document.getElementById('page-' + pageId);
    if (page) page.setAttribute('data-active', '');
  }

  function showNav(show) {
    const nav = document.getElementById('nav');
    if (nav) nav.classList.toggle('hidden', !show);
  }

  function route() {
    const { path, id } = getHashRoute();
    const loggedIn = isLoggedIn();

    if (path === 'login' || path === 'register') {
      if (loggedIn) {
        setHash('movies');
        return;
      }
      showPage(path);
      showNav(false);
      return;
    }

    if (!loggedIn) {
      setHash('login');
      return;
    }

    showNav(true);
    if (path === 'movies') {
      showPage('movies');
      loadMovies();
    } else if (path === 'movie' && id) {
      showPage('movie');
      loadMovieAndReviews(id);
    } else {
      setHash('movies');
    }
  }

  window.addEventListener('hashchange', route);
  window.addEventListener('load', route);

  // --- Auth ---
  const formLogin = document.getElementById('form-login');
  const formRegister = document.getElementById('form-register');
  const loginError = document.getElementById('login-error');
  const registerError = document.getElementById('register-error');
  const btnLogout = document.getElementById('btn-logout');

  if (formLogin) {
    formLogin.addEventListener('submit', async (e) => {
      e.preventDefault();
      loginError.classList.add('hidden');
      const username = document.getElementById('login-username').value.trim();
      const password = document.getElementById('login-password').value;
      try {
        const data = await api('api/Auth/Login', {
          method: 'POST',
          body: JSON.stringify({ username, password }),
        });
        if (data?.token) {
          setToken(data.token);
          setHash('movies');
        } else {
          loginError.textContent = data?.message || 'Login failed';
          loginError.classList.remove('hidden');
        }
      } catch (err) {
        loginError.textContent = err.data?.message || err.message || 'Login failed';
        loginError.classList.remove('hidden');
      }
    });
  }

  if (formRegister) {
    formRegister.addEventListener('submit', async (e) => {
      e.preventDefault();
      registerError.classList.add('hidden');
      const username = document.getElementById('reg-username').value.trim();
      const email = document.getElementById('reg-email').value.trim();
      const password = document.getElementById('reg-password').value;
      try {
        const data = await api('api/Auth/Register', {
          method: 'POST',
          body: JSON.stringify({ username, email, password }),
        });
        if (data?.token) {
          setToken(data.token);
          setHash('movies');
        } else {
          registerError.textContent = data?.message || 'Registration failed';
          registerError.classList.remove('hidden');
        }
      } catch (err) {
        registerError.textContent = err.data?.message || err.message || 'Registration failed';
        registerError.classList.remove('hidden');
      }
    });
  }

  if (btnLogout) {
    btnLogout.addEventListener('click', () => {
      setToken(null);
      setHash('login');
    });
  }

  // --- Movies list ---
  let sortDesc = true;

  async function loadMovies() {
    const listEl = document.getElementById('movies-list');
    const emptyEl = document.getElementById('movies-empty');
    const errEl = document.getElementById('movies-error');
    const name = document.getElementById('movies-search')?.value?.trim() || '';
    const sortBy = document.getElementById('movies-sort')?.value || 'name';
    const params = new URLSearchParams();
    params.set('PageNumber', '1');
    params.set('PageSize', '50');
    params.set('IsDescending', sortDesc ? 'true' : 'false');
    if (name) params.set('Name', name);
    if (sortBy) params.set('SortBy', sortBy);

    emptyEl?.classList.add('hidden');
    errEl?.classList.add('hidden');
    // Show loading skeletons
    if (listEl) {
      listEl.innerHTML = Array(6).fill('').map(() => `
        <div class="rounded-2xl bg-slate-900/50 border border-white/10 p-5">
          <div class="skeleton h-6 w-3/4 mb-3"></div>
          <div class="skeleton h-4 w-full mb-2"></div>
          <div class="skeleton h-4 w-2/3 mb-4"></div>
          <div class="flex justify-between">
            <div class="skeleton h-5 w-20"></div>
            <div class="skeleton h-4 w-16"></div>
          </div>
        </div>
      `).join('');
    }

    try {
      const movies = await api('api/Movies/GetAllMoviesWithQuery?' + params.toString());
      if (!movies?.length) {
        if (emptyEl) {
          emptyEl.textContent = 'No movies found.';
          emptyEl.classList.remove('hidden');
        }
        return;
      }
      listEl.innerHTML = movies
        .map(
          (m, index) => `
        <a href="#/movie/${m.id}" class="movie-card block rounded-2xl bg-slate-900/50 border border-white/10 p-5 backdrop-blur-sm group" style="animation: fadeInUp 0.5s ease-out forwards; animation-delay: ${index * 50}ms; opacity: 0;">
          <div class="flex items-start justify-between gap-3 mb-2">
            <h3 class="font-display font-semibold text-lg text-white group-hover:text-brand-300 transition-colors line-clamp-1">${escapeHtml(m.title)}</h3>
            <span class="shrink-0 text-xs font-medium px-2 py-1 rounded-full bg-white/5 text-slate-400 border border-white/5">${escapeHtml(m.releaseYear)}</span>
          </div>
          <p class="text-slate-400 text-sm mt-2 line-clamp-2 leading-relaxed">${escapeHtml(m.description || 'No description available')}</p>
          <div class="mt-4 flex items-center justify-between">
            <div class="flex items-center gap-1.5">
              <span class="star-rating">${generateStars(m.averageRating)}</span>
              <span class="text-brand-400 font-semibold text-sm ml-1">${formatRating(m.averageRating)}</span>
            </div>
            <span class="text-slate-500 text-xs flex items-center gap-1">
              <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 12h.01M12 12h.01M16 12h.01M21 12c0 4.418-4.03 8-9 8a9.863 9.863 0 01-4.255-.949L3 20l1.395-3.72C3.512 15.042 3 13.574 3 12c0-4.418 4.03-8 9-8s9 3.582 9 8z"/></svg>
              ${m.reviewCount} ${m.reviewCount === 1 ? 'review' : 'reviews'}
            </span>
          </div>
        </a>
      `
        )
        .join('');
    } catch (err) {
      if (err.status === 401) {
        setToken(null);
        setHash('login');
        return;
      }
      if (errEl) {
        errEl.textContent = err.data?.message || err.message || 'Failed to load movies';
        errEl.classList.remove('hidden');
      }
    }
  }

  const moviesSearch = document.getElementById('movies-search');
  const moviesSort = document.getElementById('movies-sort');
  const moviesSortDir = document.getElementById('movies-sort-dir');
  if (moviesSearch) moviesSearch.addEventListener('input', debounce(loadMovies, 300));
  if (moviesSort) moviesSort.addEventListener('change', loadMovies);
  if (moviesSortDir) {
    moviesSortDir.addEventListener('click', () => {
      sortDesc = !sortDesc;
      moviesSortDir.textContent = sortDesc ? '↓' : '↑';
      loadMovies();
    });
  }

  // --- Add movie modal ---
  const modalAddMovie = document.getElementById('modal-add-movie');
  const formAddMovie = document.getElementById('form-add-movie');
  const addMovieError = document.getElementById('add-movie-error');
  const btnAddMovie = document.getElementById('btn-add-movie');
  const modalAddMovieCancel = document.getElementById('modal-add-movie-cancel');

  function openModalAddMovie() {
    if (formAddMovie) formAddMovie.reset();
    if (addMovieError) { addMovieError.classList.add('hidden'); addMovieError.textContent = ''; }
    if (modalAddMovie) { modalAddMovie.classList.remove('hidden'); modalAddMovie.classList.add('flex'); }
  }
  function closeModalAddMovie() {
    if (modalAddMovie) { modalAddMovie.classList.add('hidden'); modalAddMovie.classList.remove('flex'); }
  }
  if (btnAddMovie) btnAddMovie.addEventListener('click', openModalAddMovie);
  if (modalAddMovieCancel) modalAddMovieCancel.addEventListener('click', closeModalAddMovie);
  if (modalAddMovie) modalAddMovie.addEventListener('click', (e) => { if (e.target === modalAddMovie) closeModalAddMovie(); });
  if (formAddMovie) {
    formAddMovie.addEventListener('submit', async (e) => {
      e.preventDefault();
      addMovieError?.classList.add('hidden');
      const title = document.getElementById('add-movie-title').value.trim();
      const description = document.getElementById('add-movie-description').value.trim();
      const releaseYear = parseInt(document.getElementById('add-movie-year').value, 10);
      try {
        await api('api/Movies/AddMovie', {
          method: 'POST',
          body: JSON.stringify({ title, description, releaseYear }),
        });
        closeModalAddMovie();
        loadMovies();
      } catch (err) {
        if (addMovieError) {
          addMovieError.textContent = err.data?.message || err.message || 'Failed to add movie';
          addMovieError.classList.remove('hidden');
        }
      }
    });
  }

  // --- Edit movie modal ---
  const modalEditMovie = document.getElementById('modal-edit-movie');
  const formEditMovie = document.getElementById('form-edit-movie');
  const editMovieError = document.getElementById('edit-movie-error');
  const btnEditMovie = document.getElementById('btn-edit-movie');
  const modalEditMovieCancel = document.getElementById('modal-edit-movie-cancel');

  function openModalEditMovie(movie) {
    if (!movie) return;
    document.getElementById('edit-movie-id').value = movie.id;
    document.getElementById('edit-movie-title').value = movie.title || '';
    document.getElementById('edit-movie-description').value = movie.description || '';
    document.getElementById('edit-movie-year').value = movie.releaseYear ?? '';
    if (editMovieError) { editMovieError.classList.add('hidden'); editMovieError.textContent = ''; }
    if (modalEditMovie) { modalEditMovie.classList.remove('hidden'); modalEditMovie.classList.add('flex'); }
  }
  function closeModalEditMovie() {
    if (modalEditMovie) { modalEditMovie.classList.add('hidden'); modalEditMovie.classList.remove('flex'); }
  }
  if (modalEditMovieCancel) modalEditMovieCancel.addEventListener('click', closeModalEditMovie);
  if (modalEditMovie) modalEditMovie.addEventListener('click', (e) => { if (e.target === modalEditMovie) closeModalEditMovie(); });
  if (formEditMovie) {
    formEditMovie.addEventListener('submit', async (e) => {
      e.preventDefault();
      editMovieError?.classList.add('hidden');
      const id = document.getElementById('edit-movie-id').value;
      const title = document.getElementById('edit-movie-title').value.trim();
      const description = document.getElementById('edit-movie-description').value.trim();
      const releaseYear = parseInt(document.getElementById('edit-movie-year').value, 10);
      try {
        await api(`api/Movies/UpdateMovie/${id}`, {
          method: 'PUT',
          body: JSON.stringify({ title, description, releaseYear }),
        });
        closeModalEditMovie();
        loadMovieAndReviews(id);
      } catch (err) {
        if (editMovieError) {
          editMovieError.textContent = err.data?.message || err.message || 'Failed to update movie';
          editMovieError.classList.remove('hidden');
        }
      }
    });
  }

  // --- Movie detail + reviews ---
  let currentMovieId = null;
  let currentMovie = null;

  async function loadMovieAndReviews(id) {
    currentMovieId = id;
    currentMovie = null;
    const titleEl = document.getElementById('movie-title');
    const metaEl = document.getElementById('movie-meta');
    const descEl = document.getElementById('movie-description');
    const statsEl = document.getElementById('movie-stats');
    const reviewsList = document.getElementById('reviews-list');
    const reviewsEmpty = document.getElementById('reviews-empty');
    const formReview = document.getElementById('form-review');

    if (formReview) formReview.reset();
    if (reviewsList) reviewsList.innerHTML = '';
    if (reviewsEmpty) reviewsEmpty.classList.add('hidden');

    try {
      const movie = await api(`api/Movies/GetById/${id}`);
      if (!movie) {
        setHash('movies');
        return;
      }
      currentMovie = movie;
      titleEl.textContent = movie.title || 'Movie';
      metaEl.innerHTML = `<span class="inline-flex items-center gap-1.5"><svg class="w-4 h-4 text-slate-400" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"/></svg>Released ${movie.releaseYear}</span>`;
      descEl.textContent = movie.description || 'No description available.';
      statsEl.innerHTML = `
        <div class="flex items-center gap-1.5 px-3 py-1.5 rounded-full bg-white/5 border border-white/10">
          <span class="star-rating">${generateStars(movie.averageRating)}</span>
          <span class="text-brand-300 font-semibold ml-1">${formatRating(movie.averageRating)}</span>
        </div>
        <div class="flex items-center gap-1.5 px-3 py-1.5 rounded-full bg-white/5 border border-white/10 text-slate-300">
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 8h2a2 2 0 012 2v6a2 2 0 01-2 2h-2v4l-4-4H9a1.994 1.994 0 01-1.414-.586m0 0L11 14h4a2 2 0 002-2V6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2v4l.586-.586z"/></svg>
          <span>${movie.reviewCount} ${movie.reviewCount === 1 ? 'review' : 'reviews'}</span>
        </div>
      `;

      const reviews = await api(`api/Reviews/GetByMovieId/${id}`);
      const currentUserId = getCurrentUserId();
      if (reviews?.length) {
        reviewsList.innerHTML = reviews
          .map(
            (r, index) => {
              const canDelete = currentUserId != null && r.userId === currentUserId;
              return `
          <div class="review-card rounded-xl bg-slate-800/50 border border-white/10 p-4 backdrop-blur-sm" style="animation: fadeInUp 0.4s ease-out forwards; animation-delay: ${index * 50}ms; opacity: 0;">
            <div class="flex justify-between items-start gap-3">
              <div class="flex-1 min-w-0">
                <p class="text-slate-200 leading-relaxed">${escapeHtml(r.content)}</p>
                <div class="flex items-center gap-2 mt-3">
                  <div class="w-6 h-6 rounded-full bg-gradient-to-br from-brand-400 to-indigo-500 flex items-center justify-center text-xs font-bold text-white">${escapeHtml((r.userName || 'A').charAt(0).toUpperCase())}</div>
                  <span class="text-slate-400 text-sm font-medium">${escapeHtml(r.userName || 'Anonymous')}</span>
                </div>
              </div>
              <div class="flex flex-col items-end gap-2 shrink-0">
                <div class="flex items-center gap-1 px-2.5 py-1 rounded-full bg-brand-500/20 border border-brand-400/30">
                  <span class="text-amber-400 text-sm">★</span>
                  <span class="text-brand-300 font-semibold text-sm">${r.rating}/10</span>
                </div>
                ${canDelete ? `<button type="button" data-delete-review="${r.id}" class="text-rose-400 hover:text-rose-300 text-xs font-medium transition-colors hover:underline">Delete</button>` : ''}
              </div>
            </div>
          </div>
        `;
            }
          )
          .join('');
      } else {
        if (reviewsEmpty) {
          reviewsEmpty.textContent = 'No reviews yet.';
          reviewsEmpty.classList.remove('hidden');
        }
      }
    } catch (err) {
      if (err.status === 401) {
        setToken(null);
        setHash('login');
        return;
      }
      if (titleEl) titleEl.textContent = 'Error loading movie';
    }
  }

  if (document.getElementById('btn-edit-movie')) {
    document.getElementById('btn-edit-movie').addEventListener('click', () => {
      if (currentMovie) openModalEditMovie(currentMovie);
    });
  }
  if (document.getElementById('btn-delete-movie')) {
    document.getElementById('btn-delete-movie').addEventListener('click', async () => {
      if (!currentMovieId || !confirm('Delete this movie? This cannot be undone.')) return;
      try {
        await api(`api/Movies/DeleteMovie/${currentMovieId}`, { method: 'DELETE' });
        setHash('movies');
      } catch (err) {
        alert(err.data?.message || err.message || 'Failed to delete movie');
      }
    });
  }

  document.getElementById('reviews-list')?.addEventListener('click', async (e) => {
    const btn = e.target.closest('[data-delete-review]');
    if (!btn) return;
    const reviewId = btn.getAttribute('data-delete-review');
    if (!reviewId || !currentMovieId || !confirm('Delete this review?')) return;
    try {
      await api(`api/Reviews/DeleteReview/${reviewId}`, { method: 'DELETE' });
      loadMovieAndReviews(currentMovieId);
    } catch (err) {
      alert(err.data?.message || err.message || 'Failed to delete review');
    }
  });

  const formReview = document.getElementById('form-review');
  if (formReview) {
    formReview.addEventListener('submit', async (e) => {
      e.preventDefault();
      const content = document.getElementById('review-content').value.trim();
      const rating = parseInt(document.getElementById('review-rating').value, 10);
      const movieId = currentMovieId;
      if (!movieId || !content) return;
      try {
        await api('api/Reviews/AddReview', {
          method: 'POST',
          body: JSON.stringify({ content, rating, movieId }),
        });
        formReview.reset();
        loadMovieAndReviews(movieId);
      } catch (err) {
        alert(err.data?.message || err.message || 'Failed to add review');
      }
    });
  }

  function escapeHtml(s) {
    if (s == null) return '';
    const div = document.createElement('div');
    div.textContent = s;
    return div.innerHTML;
  }

  function formatRating(n) {
    if (n == null || isNaN(n)) return '—';
    return Number(n).toFixed(1);
  }

  function generateStars(rating) {
    if (rating == null || isNaN(rating)) rating = 0;
    const normalizedRating = Math.round(rating / 2); // Convert 1-10 scale to 1-5 stars
    let stars = '';
    for (let i = 1; i <= 5; i++) {
      if (i <= normalizedRating) {
        stars += '<span class="star filled">★</span>';
      } else {
        stars += '<span class="star">★</span>';
      }
    }
    return stars;
  }

  function debounce(fn, ms) {
    let t;
    return function () {
      clearTimeout(t);
      t = setTimeout(() => fn.apply(this, arguments), ms);
    };
  }
})();
