(function() {
    const exp = localStorage.getItem('TokenExpiration');
    const timerEl = document.getElementById('timer');
    if (!exp || !timerEl) return;
    
    function updateTimer() {
      const diffMs = new Date(exp) - new Date();
      if (diffMs <= 0) {
        timerEl.textContent = '00:00';
        localStorage.removeItem('Bearer_Token');
        localStorage.removeItem('TokenExpiration');
        window.location = '@Url.Action("Login","LoginMvc")';
        return;
      }
      const totalSec = Math.floor(diffMs / 1000);
      const m = Math.floor(totalSec / 60).toString().padStart(2, '0');
      const s = (totalSec % 60).toString().padStart(2, '0');
      timerEl.textContent = `${m}:${s}`;
    }

    updateTimer();
    setInterval(updateTimer, 1000);
  })();