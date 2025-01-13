import React, { createContext, useState, useEffect, useCallback, ReactNode } from 'react';
import AuthService from '@/services/AuthService';
import { Person } from '@/types';

type ContextUser = {
  user: Person | undefined;
  login: (email: string, password: string) => Promise<void>;
  logout: () => Promise<void>;
  register: (email: string, password: string, person: Person) => Promise<void>;
  checkEmail: (email: string) => Promise<void>;
  changePassword: (oldPassword: string, newPassword: string) => Promise<void>;
  setUser: (user: Person | undefined) => void;
  isLoading: boolean;
};

type AuthProviderProps = {
  children: ReactNode;
};

const AuthContext = createContext<ContextUser | undefined>(undefined);

export const AuthProvider: React.FC<AuthProviderProps> = ({ children }) => {
  const [user, setUser] = useState<Person | undefined>(undefined);
  const [isLoading, setIsLoading] = useState<boolean>(true);

  const login = async (email: string, password: string) => {
    const response = await AuthService.login({ email, password });
    setUser(response.person);
  };

  const logout = async () => {
    await AuthService.logout();
    setUser(undefined);
  };

  const register = async (email: string, password: string, person: Person) => {
    const response = await AuthService.register({ email, password, person });
    setUser(response.person);
  };

  const checkEmail = async (email: string) => {
    await AuthService.checkEmail(email);
  };

  const changePassword = async (oldPassword: string, newPassword: string) => {
    await AuthService.changePassword({ oldPassword, newPassword });
  };

  const verifyUser = useCallback(async () => {
    try {
      const response = await AuthService.verifyToken();
      setUser(response.person);
    } catch (error) {
      console.error('Token verification failed:', error);
    } finally {
      setIsLoading(false);
    }
  }, []);

  useEffect(() => {
    verifyUser();
  }, [verifyUser]);

  return (
    <AuthContext.Provider
      value={{
        user,
        login,
        logout,
        register,
        checkEmail,
        changePassword,
        setUser,
        isLoading,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = React.useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};