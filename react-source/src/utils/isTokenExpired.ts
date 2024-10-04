export const isTokenExpired = (token: string) => {
  if (!token) return true;
  const payload = JSON.parse(atob(token.split(".")[1]));
  let res = payload.exp * 1000 < Date.now();
  return res;
};
