// Scroll-based header show/hide
(function () {
  const header = document.querySelector('.site-header');
  if (!header) return;

  let lastScrollY = window.scrollY;
  let ticking = false;

  function onScroll() {
    const currentY = window.scrollY;

    if (currentY <= 0) {
      // At the very top — always show
      header.classList.remove('header-hidden');
    } else if (currentY > lastScrollY) {
      // Scrolling DOWN — hide
      header.classList.add('header-hidden');
    } else {
      // Scrolling UP — show
      header.classList.remove('header-hidden');
    }

    lastScrollY = currentY;
    ticking = false;
  }

  window.addEventListener('scroll', function () {
    if (!ticking) {
      requestAnimationFrame(onScroll);
      ticking = true;
    }
  }, { passive: true });
})();
