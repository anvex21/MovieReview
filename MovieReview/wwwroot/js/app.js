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
    if (listEl) listEl.innerHTML = '';

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
          (m) => `
        <a href="#/movie/${m.id}" class="block rounded-xl bg-surface-800/80 border border-surface-600/50 p-5 hover:border-brand-500/50 transition group">
          <h3 class="font-display font-semibold text-lg text-white group-hover:text-brand-400 transition">${escapeHtml(m.title)}</h3>
          <p class="text-zinc-500 text-sm mt-1">${escapeHtml(m.releaseYear)}</p>
          <p class="text-zinc-400 text-sm mt-2 line-clamp-2">${escapeHtml(m.description || '')}</p>
          <div class="mt-3 flex items-center gap-2 text-sm">
            <span class="text-brand-400 font-medium">★ ${formatRating(m.averageRating)}</span>
            <span class="text-zinc-500">${m.reviewCount} reviews</span>
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
      metaEl.textContent = `Released ${movie.releaseYear}`;
      descEl.textContent = movie.description || 'No description.';
      statsEl.innerHTML = `
        <span>★ ${formatRating(movie.averageRating)}</span>
        <span>${movie.reviewCount} reviews</span>
      `;

      const reviews = await api(`api/Reviews/GetByMovieId/${id}`);
      const currentUserId = getCurrentUserId();
      if (reviews?.length) {
        reviewsList.innerHTML = reviews
          .map(
            (r) => {
              const canDelete = currentUserId != null && r.userId === currentUserId;
              return `
          <div class="rounded-lg bg-surface-700/50 border border-surface-600/50 p-4">
            <div class="flex justify-between items-start gap-2">
              <p class="text-zinc-300">${escapeHtml(r.content)}</p>
              <div class="flex items-center gap-2 shrink-0">
                <span class="text-brand-400 font-medium">${r.rating}/10</span>
                ${canDelete ? `<button type="button" data-delete-review="${r.id}" class="text-red-400 hover:text-red-300 text-xs font-medium transition">Delete</button>` : ''}
              </div>
            </div>
            <p class="text-zinc-500 text-sm mt-2">${escapeHtml(r.userName || 'Anonymous')}</p>
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

  function debounce(fn, ms) {
    let t;
    return function () {
      clearTimeout(t);
      t = setTimeout(() => fn.apply(this, arguments), ms);
    };
  }
})();
