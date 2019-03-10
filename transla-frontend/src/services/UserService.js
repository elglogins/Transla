export const userService = {
  isLoggedIn,
  get,
  logout,
  getSavedUsers,
  create,
  remove,
  login,
  getApiKey,
  getBaseUrl
};

function getApiKey() {
  if (!isLoggedIn()) {
    return null;
  }

  return get().apiKey;
}

function getBaseUrl() {
  if (!isLoggedIn()) {
    return null;
  }

  return get().baseUrl;
}

function isLoggedIn() {
  if (sessionStorage.getItem("user") === null) {
    return false;
  }

  return true;
}

function login(baseUrl, apiKey) {
  sessionStorage.setItem(
    "user",
    JSON.stringify({
      baseUrl,
      apiKey
    })
  );
}

function get() {
  if (!isLoggedIn()) {
    return null;
  }
  return JSON.parse(sessionStorage.getItem("user"));
}

function logout() {
  if (isLoggedIn()) {
    sessionStorage.removeItem("user");
  }
}

function getSavedUsers() {
  var users = JSON.parse(localStorage.getItem("savedUsers"));
  if (users) {
    return users;
  } else {
    return [];
  }
}

function create(baseUrl, apiKey) {
  var users = getSavedUsers();
  users.push({
    baseUrl,
    apiKey
  });
  localStorage.setItem("savedUsers", JSON.stringify(users));
}

function remove(baseUrl) {
  var users = getSavedUsers();
  var matchingUser = users.find(f => f.baseUrl === baseUrl);
  if (matchingUser) {
    var index = users.indexOf(matchingUser);
    users.splice(index, 1);
    localStorage.setItem("savedUsers", JSON.stringify(users));
  }
}
