(function () {
  'use strict';

  const mountNode = document.getElementById('react-movies-insights');
  if (!mountNode || !window.React || !window.ReactDOM || !window.ReactDOM.createRoot) return;

  const React = window.React;
  const ReactDOM = window.ReactDOM;
  const h = React.createElement;

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

  ReactDOM.createRoot(mountNode).render(h(MoviesInsights));
})();
