(function () {
  'use strict';

  if (!window.React || !window.ReactDOM || !window.ReactDOM.createRoot) return;

  const React = window.React;
  const ReactDOM = window.ReactDOM;
  const h = React.createElement;
  const TOKEN_KEY = 'moviereview_token';

  function getToken() {
    return localStorage.getItem(TOKEN_KEY);
  }

  function getCurrentUserId() {
    const token = getToken();
    if (!token) return null;
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const sub = payload.sub;
      const userId = sub ? parseInt(sub, 10) : null;
      return Number.isFinite(userId) ? userId : null;
    } catch (_) {
      return null;
    }
  }

  async function api(path, options) {
    const headers = {
      'Content-Type': 'application/json',
      ...(options?.headers || {}),
    };
    const token = getToken();
    if (token) headers.Authorization = 'Bearer ' + token;

    const res = await fetch(path, {
      ...options,
      headers,
    });
    const text = await res.text();
    let data = null;
    try {
      data = text ? JSON.parse(text) : null;
    } catch (_) {}

    if (!res.ok) {
      const message = (data && (data.message ?? data.Message)) || res.statusText || 'Request failed';
      const err = new Error(message);
      err.status = res.status;
      err.data = data;
      throw err;
    }
    return data;
  }

  function asNumber(value) {
    const n = Number(value);
    return Number.isFinite(n) ? n : null;
  }

  function getRating(movie) {
    return asNumber(movie?.averageRating ?? movie?.AverageRating);
  }

  function getReviewCount(movie) {
    return asNumber(movie?.reviewCount ?? movie?.ReviewCount) ?? 0;
  }

  function moviesSummary(movies) {
    const rated = movies.map(getRating).filter((n) => n !== null);
    const totalReviews = movies.reduce((sum, movie) => sum + getReviewCount(movie), 0);
    const avgRating = rated.length ? rated.reduce((sum, n) => sum + n, 0) / rated.length : null;
    const topMovie = movies
      .map((movie) => ({ movie, rating: getRating(movie) }))
      .filter((row) => row.rating !== null)
      .sort((a, b) => b.rating - a.rating)[0]?.movie;

    return {
      count: movies.length,
      avgRating,
      totalReviews,
      topMovieTitle: topMovie?.title ?? topMovie?.Title ?? 'N/A',
    };
  }

  function StatChip(props) {
    return h(
      'div',
      { className: 'rounded-xl border border-white/10 bg-slate-900/50 px-3 py-2' },
      h('p', { className: 'text-[11px] uppercase tracking-wide text-slate-400' }, props.label),
      h('p', { className: 'text-sm font-semibold text-white mt-0.5' }, props.value)
    );
  }

  function MoviesInsights() {
    const [state, setState] = React.useState({
      status: 'idle',
      movies: [],
      query: '',
      sortBy: 'name',
      sortDesc: true,
      error: null,
    });

    React.useEffect(() => {
      function onMoviesState(event) {
        if (!event?.detail) return;
        const detail = event.detail;
        setState((prev) => ({
          ...prev,
          ...detail,
          movies: Array.isArray(detail.movies) ? detail.movies : prev.movies,
        }));
      }
      window.addEventListener('movies:state', onMoviesState);
      return () => window.removeEventListener('movies:state', onMoviesState);
    }, []);

    if (state.status === 'idle') return null;

    if (state.status === 'error') {
      return h(
        'div',
        { className: 'mb-4 rounded-2xl border border-rose-400/30 bg-rose-500/10 p-3 text-sm text-rose-200' },
        'Statistics unavailable: ',
        state.error || 'Failed to load movies'
      );
    }

    const summary = moviesSummary(state.movies);

    return h(
      'section',
      { className: 'rounded-2xl border border-white/10 bg-slate-900/40 p-3 sm:p-4 backdrop-blur-sm' },
      h(
        'div',
        { className: 'flex flex-wrap items-center justify-between gap-3 mb-3' },
        h(
          'p',
          { className: 'text-sm text-slate-300' },
          state.status === 'loading' ? 'Updating insights…' : 'Statistics: '
        ),
        h(
          'p',
          { className: 'text-xs text-slate-400' },
          'Query: ',
          state.query || 'all',
          ' • Sort: ',
          state.sortBy,
          ' ',
          state.sortDesc ? 'desc' : 'asc'
        )
      ),
      h(
        'div',
        { className: 'grid grid-cols-2 lg:grid-cols-4 gap-2.5' },
        h(StatChip, { label: 'Movies', value: String(summary.count) }),
        h(StatChip, {
          label: 'Avg Rating',
          value: summary.avgRating === null ? 'N/A' : summary.avgRating.toFixed(1) + '/10',
        }),
        h(StatChip, { label: 'Total Reviews', value: String(summary.totalReviews) }),
        h(StatChip, { label: 'Top Movie', value: summary.topMovieTitle }),
      )
    );
  }

  function MyReviewsDashboard() {
    const [state, setState] = React.useState({
      loading: false,
      error: null,
      reviews: [],
      moviesById: {},
      query: '',
      minRating: 1,
      sortBy: 'newest',
    });

    const loadData = React.useCallback(async () => {
      const userId = getCurrentUserId();
      if (!userId) {
        setState((prev) => ({ ...prev, loading: false, error: 'User not authenticated', reviews: [] }));
        return;
      }

      setState((prev) => ({ ...prev, loading: true, error: null }));
      try {
        const [reviews, movies] = await Promise.all([
          api('api/Reviews/GetByUserId/' + userId),
          api('api/Movies/GetAllMoviesWithQuery?PageNumber=1&PageSize=300&IsDescending=true&SortBy=name'),
        ]);
        const moviesById = (Array.isArray(movies) ? movies : []).reduce((acc, movie) => {
          acc[movie.id] = movie;
          return acc;
        }, {});
        setState((prev) => ({
          ...prev,
          loading: false,
          error: null,
          reviews: Array.isArray(reviews) ? reviews : [],
          moviesById,
        }));
      } catch (err) {
        setState((prev) => ({
          ...prev,
          loading: false,
          error: err.message || 'Failed to load your reviews',
        }));
      }
    }, []);

    React.useEffect(() => {
      function maybeLoad() {
        const hash = window.location.hash || '#/movies';
        if (hash.startsWith('#/my-reviews')) loadData();
      }

      maybeLoad();
      window.addEventListener('hashchange', maybeLoad);
      return () => window.removeEventListener('hashchange', maybeLoad);
    }, [loadData]);

    async function onDelete(reviewId) {
      if (!reviewId || !confirm('Delete this review?')) return;
      try {
        await api('api/Reviews/DeleteReview/' + reviewId, { method: 'DELETE' });
        setState((prev) => ({
          ...prev,
          reviews: prev.reviews.filter((review) => review.id !== reviewId),
        }));
      } catch (err) {
        setState((prev) => ({ ...prev, error: err.message || 'Failed to delete review' }));
      }
    }

    const normalizedQuery = state.query.trim().toLowerCase();
    const filteredReviews = state.reviews
      .filter((review) => review.rating >= state.minRating)
      .filter((review) => {
        if (!normalizedQuery) return true;
        const movie = state.moviesById[review.movieId];
        const movieTitle = (movie?.title || '').toLowerCase();
        return review.content.toLowerCase().includes(normalizedQuery) || movieTitle.includes(normalizedQuery);
      })
      .slice()
      .sort((a, b) => {
        if (state.sortBy === 'rating-desc') return b.rating - a.rating;
        if (state.sortBy === 'rating-asc') return a.rating - b.rating;
        return b.id - a.id;
      });

    const avgRating = filteredReviews.length
      ? filteredReviews.reduce((sum, review) => sum + review.rating, 0) / filteredReviews.length
      : null;

    return h(
      'section',
      { className: 'rounded-3xl border border-white/10 bg-slate-900/35 backdrop-blur-sm p-4 sm:p-5' },
      h(
        'div',
        { className: 'grid grid-cols-1 sm:grid-cols-3 gap-2.5 mb-4' },
        h(StatChip, { label: 'Reviews', value: String(filteredReviews.length) }),
        h(StatChip, { label: 'Average', value: avgRating == null ? 'N/A' : avgRating.toFixed(1) + '/10' }),
        h(StatChip, {
          label: 'High Rated',
          value: String(filteredReviews.filter((review) => review.rating >= 8).length),
        }),
      ),
      h(
        'div',
        { className: 'grid grid-cols-1 sm:grid-cols-3 gap-2.5 mb-4' },
        h('input', {
          value: state.query,
          onChange: (e) => setState((prev) => ({ ...prev, query: e.target.value })),
          placeholder: 'Search by movie or review text',
          className:
            'w-full rounded-xl bg-white/5 border border-white/12 text-slate-50 placeholder-slate-400/70 px-3 py-2 text-sm focus:border-brand-400/90 focus:ring-1 focus:ring-brand-400/70 outline-none',
        }),
        h(
          'select',
          {
            value: String(state.minRating),
            onChange: (e) => setState((prev) => ({ ...prev, minRating: parseInt(e.target.value, 10) || 1 })),
            className:
              'rounded-xl bg-white/5 border border-white/12 text-slate-50 px-3 py-2 text-sm focus:border-brand-400/90 focus:ring-1 focus:ring-brand-400/70 outline-none',
          },
          h('option', { value: '1' }, 'Min rating: 1+'),
          h('option', { value: '5' }, 'Min rating: 5+'),
          h('option', { value: '8' }, 'Min rating: 8+'),
          h('option', { value: '10' }, 'Min rating: 10')
        ),
        h(
          'select',
          {
            value: state.sortBy,
            onChange: (e) => setState((prev) => ({ ...prev, sortBy: e.target.value })),
            className:
              'rounded-xl bg-white/5 border border-white/12 text-slate-50 px-3 py-2 text-sm focus:border-brand-400/90 focus:ring-1 focus:ring-brand-400/70 outline-none',
          },
          h('option', { value: 'newest' }, 'Sort: newest'),
          h('option', { value: 'rating-desc' }, 'Sort: rating high-low'),
          h('option', { value: 'rating-asc' }, 'Sort: rating low-high'),
        )
      ),
      state.loading
        ? h('p', { className: 'text-slate-300 text-sm py-6' }, 'Loading your reviews...')
        : null,
      state.error
        ? h('p', { className: 'text-rose-300 text-sm py-2' }, state.error)
        : null,
      !state.loading && !filteredReviews.length
        ? h(
            'p',
            { className: 'text-slate-400 text-sm py-8 text-center border border-dashed border-white/10 rounded-2xl' },
            'No reviews match your current filters.'
          )
        : null,
      h(
        'div',
        { className: 'space-y-3' },
        filteredReviews.map((review) => {
          const movie = state.moviesById[review.movieId];
          const movieTitle = movie?.title || 'Movie #' + review.movieId;
          return h(
            'article',
            {
              key: String(review.id),
              className: 'rounded-2xl border border-white/10 bg-slate-900/60 p-4',
            },
            h(
              'div',
              { className: 'flex items-start justify-between gap-3 mb-2' },
              h(
                'div',
                { className: 'min-w-0' },
                h(
                  'a',
                  {
                    href: '#/movie/' + review.movieId,
                    className: 'text-brand-300 hover:text-brand-200 text-sm font-semibold transition-colors',
                  },
                  movieTitle
                ),
                h('p', { className: 'text-xs text-slate-400 mt-0.5' }, 'Review #', String(review.id))
              ),
              h(
                'span',
                { className: 'rounded-full px-2.5 py-1 text-xs border border-brand-400/30 bg-brand-500/20 text-brand-300 font-semibold' },
                String(review.rating),
                '/10'
              )
            ),
            h('p', { className: 'text-slate-200 text-sm leading-relaxed' }, review.content),
            h(
              'div',
              { className: 'mt-3 flex items-center justify-end gap-2' },
              h(
                'button',
                {
                  type: 'button',
                  onClick: function () {
                    onDelete(review.id);
                  },
                  className:
                    'rounded-lg border border-rose-400/40 bg-rose-500/15 text-rose-300 hover:bg-rose-500/25 px-2.5 py-1 text-xs font-medium transition-colors',
                },
                'Delete'
              )
            )
          );
        })
      )
    );
  }

  function mountMoviesInsights() {
    const mountNode = document.getElementById('react-movies-insights');
    if (!mountNode) return;
    ReactDOM.createRoot(mountNode).render(h(MoviesInsights));
  }

  function mountMyReviewsDashboard() {
    const mountNode = document.getElementById('react-my-reviews-dashboard');
    if (!mountNode) return;
    ReactDOM.createRoot(mountNode).render(h(MyReviewsDashboard));
  }

  mountMoviesInsights();
  mountMyReviewsDashboard();
})();
