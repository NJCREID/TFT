import React, { Dispatch, SetStateAction, createContext, useContext, useEffect, useState } from "react";
import fetchRequest from "../common/api";
import { ENDPOINT } from "../common/endpoints";
import { useNavigate } from "react-router-dom";
import { isTokenExpired } from "../utils/isTokenExpired";

export interface StoredUser {
  user: User;
  token: string;
}

export interface User {
  id: number;
  username: string;
  name: string;
  upVotesCount: number;
  downVotesCount: number;
  commentsCount: number;
  guidesCount: number;
}

export interface UserInput {
  username: string;
  name: string;
  email: string;
  password: string;
}

export interface UserLogin {
  email: string;
  password: string;
}

interface UserContextProps {
  user: StoredUser | null;
  loading: boolean;
  err: Array<string> | null;
  setErr: Dispatch<SetStateAction<Array<string> | null>>;
  login: (userInput: UserLogin) => Promise<void>;
  logout: () => void;
  register: (userInput: UserInput) => Promise<void>;
}

const AuthContext = createContext<UserContextProps | null>(null);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const storedUser = (() => {
    try {
      return JSON.parse(localStorage.getItem("user") ?? "null");
    } catch {
      return null;
    }
  })();

  const [user, setUser] = useState<StoredUser | null>(storedUser);
  const [loading, setLoading] = useState(false);
  const [err, setErr] = useState<Array<string> | null>(null);
  const navigate = useNavigate();

  useEffect(() => {
    if (user && isTokenExpired(user.token)) {
      logout();
    }
  }, [user]);

  useEffect(() => {
    if (user) {
      const tokenCheckInterval = setInterval(() => {
        if (isTokenExpired(user.token)) {
          logout();
        }
      }, 60000);
      return () => clearInterval(tokenCheckInterval);
    }
  }, [user]);

  const login = async (userInput: UserLogin) => {
    try {
      setLoading(true);
      const res: StoredUser = await fetchRequest(ENDPOINT.AUTH_LOGIN, "POST", userInput);
      localStorage.setItem("user", JSON.stringify(res));
      setUser(res);
    } catch (error: any) {
      setErr([error.response?.data?.msg || "An error occurred"]);
    } finally {
      setLoading(false);
    }
  };

  const logout = () => {
    localStorage.removeItem("user");
    setUser(null);
    navigate("/sign-in");
  };

  const register = async (userInput: UserInput) => {
    try {
      await fetchRequest(ENDPOINT.AUTH_REGISTER, "POST", userInput);
      if (user) {
        setUser(null);
      }
    } catch (error: any) {
      setErr([error.response?.data?.msg || "An error occurred"]);
    }
  };

  return (
    <AuthContext.Provider value={{ user, loading, err, setErr, login, logout, register }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuthContext = (): UserContextProps => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuthContext must be used within an AuthProvider");
  }
  return context;
};
