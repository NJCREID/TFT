import { useNavigate } from "react-router-dom";
import { useAuthContext } from "../../context/authContext";
import "./register.css";
import { useEffect } from "react";
import { Link } from "react-router-dom";
import Button from "../../components/Button/Button";
const Register = () => {
  const { register, setErr, err } = useAuthContext();
  const navigate = useNavigate();

  useEffect(() => {
    setErr(null);
  }, []);

  const registerUser = async (event: React.SyntheticEvent) => {
    event.preventDefault();
    const { name, username, email, password } = event.target as typeof event.target & {
      name: HTMLInputElement;
      username: HTMLInputElement;
      email: HTMLInputElement;
      password: HTMLInputElement;
    };
    try {
      await register({
        name: name.value,
        username: username.value,
        email: email.value,
        password: password.value,
      });
      setErr(null);
      navigate("/sign-in");
    } catch (error) {
      setErr(error as any);
    }
  };
  return (
    <div className="app__register">
      <form onSubmit={registerUser} className="app__register-form">
        <article>
          <h1 className="app__register-form-heading p__opensans-title-large">Sign Up</h1>
          <div className="app__register-form-details">
            <input className="app__register-form-input" type="text" placeholder="Name" name="name" id="name" />
            <input
              className="app__register-form-input"
              type="text"
              placeholder="Username"
              name="username"
              id="username"
            />
            <input className="app__register-form-input" type="email" placeholder="Email" name="email" id="email" />
            <input
              className="app__register-form-input"
              type="password"
              placeholder="Password"
              name="password"
              id="password"
            />
            <Button type="submit">Sign Up</Button>
          </div>
          {err && (
            <div className="app__register-form-error">
              <p className="p__opensans">Error:</p>
              <ol className="app__register-form-error-list">
                {err.map((item) => (
                  <li key={item} className="p__opensans">
                    {item}
                  </li>
                ))}
              </ol>
            </div>
          )}
          <p className="app__register-form-redirect p__opensans">
            Already have an account?{" "}
            <Link to="/sign-in" className="app__register-form-redirect-link">
              Sign in here.
            </Link>
          </p>
        </article>
      </form>
    </div>
  );
};
export default Register;
