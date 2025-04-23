(function() {
    const token = localStorage.getItem('Bearer_Token');
    const exp   = localStorage.getItem('TokenExpiration');
    if (!token || !exp || new Date(exp) <= new Date()) {
      localStorage.removeItem('Bearer_Token');
      localStorage.removeItem('TokenExpiration');
      window.location = '@Url.Action("Login","LoginMvc")';
    }
  })();