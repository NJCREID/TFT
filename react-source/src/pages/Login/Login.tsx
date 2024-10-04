import { useNavigate } from "react-router-dom";
import { useAuthContext } from "../../context";
import "./login.css";
import { useEffect } from "react";
import { Link } from "react-router-dom";
import { Button } from "../../components";
const Login = () => {
  const { login, user, setErr, err } = useAuthContext();
  const navigate = useNavigate();

  useEffect(() => {
    setErr(null);
  }, []);

  useEffect(() => {
    if (user) {
      navigate("/");
    }
  });

  const authenticateUser = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    const { email, password } = event.target as typeof event.target & {
      email: HTMLInputElement;
      password: HTMLInputElement;
    };
    try {
      await login({ email: email.value, password: password.value });
      setErr(null);
    } catch (error) {
      setErr(error as any);
    }
  };
  return (
    <div className="app__login">
      <form onSubmit={authenticateUser} className="app__login-form">
        <article>
          <p className="app__login-form-heading p__opensans-title-large">Sign In</p>
          <div className="app__login-form-details">
            <input className="app__login-form-input" type="email" placeholder="Email" name="email" id="email" />
            <input
              className="app__login-form-input"
              type="password"
              placeholder="Password"
              name="password"
              id="password"
            />
            <Button type="submit" aria-label="Sign in">
              Sign In
            </Button>
          </div>
          {err && (
            <div className="app__login-form-error">
              <p className="p__opensans">Error:</p>
              <ol className="app__login-form-error-list">
                {err.map((item) => (
                  <li key={item} className="p__opensans">
                    {item}
                  </li>
                ))}
              </ol>
            </div>
          )}
          <p className="app__login-form-redirect p__opensans">
            Don't have an account?{" "}
            <Link to="/register" className="app__login-form-redirect-link">
              Sign up here.
            </Link>
          </p>
        </article>
      </form>
    </div>
  );
};
export default Login;
