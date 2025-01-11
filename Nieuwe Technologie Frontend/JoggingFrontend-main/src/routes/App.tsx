import React from "react";
import { Navigate, Outlet, Route, Routes } from "react-router-dom";
import Home from "./public/Home";
import Admin from "./admin/Admin";
import { useAuth } from "./auth/context/AuthProvider";
import Settings from "./admin/settings/Settings";
import { ThemeProvider } from "@/components/ThemeProvider";
import { Toaster } from "@/components/ui/sonner";
import Data from "./admin/data/Data";
import Profile from "./admin/settings/profile/Profile";
import Wedstrijden from "./admin/data/wedstrijden/Wedstrijden";
import Personen from "./admin/data/personen/Personen";
import { AuthRegisterForm } from "../components/forms/authRegisterForm/AuthRegisterForm";
import Auth from "./auth/Auth";
import { AuthLoginForm } from "./auth/login/AuthLoginForm";
import Wedstrijd from "./wedstrijd/Wedstrijd";
import Inschrijving from "./wedstrijd/inschrijving/Inschrijving";
import AccountPage from "./account/Account";
import Dashboard from "./account/dashboard/Dashboard";
import AdminDashboard from "./admin/dashboard/AdminDashboard";
import Account from "./admin/settings/account/Account";
import Results from "./wedstrijd/results/Results";
import AllResults from "./wedstrijd/results/AllResults";
import { ResetPassword } from "./auth/resetPassword/ResetPassword";
import { RequestPassword } from "./auth/requestPassword/RequestPassword";
import { ConfirmEmail } from "./auth/confirm/ConfirmEmail";
import NotFound from "./notFound/NotFound";
import Klassementen from "./klassementen/Klassementen";
import CookieDrawer from "@/components/CookieDrawer";
import PrivacyPolicy from "@/routes/legal/PrivacyPolicy.tsx";
import GeneralConditions from "@/routes/legal/GeneralConditions.tsx";
import DeleteAccount from "@/routes/admin/settings/account/DeleteAccount.tsx";
import Uitslagen from "./public/uitslagen/Uitslagen";
import { Clubs } from "./public/clubs/clubs";

interface ProtectedRouteProps {
  requiredRole: string;
}

const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ requiredRole }) => {
  const { user, isLoading } = useAuth();

  if (isLoading) {
    return <div>Loading...</div>;
  }

  if (!user || user.profile?.role !== requiredRole) {
    return <Navigate to="/auth/login" replace />;
  }

  return <Outlet />;
};

const UnauthenticatedRoute: React.FC = () => {
  const { user } = useAuth();

  if (user) {
    return <Navigate to="/" replace />;
  }

  return <Outlet />;
};

export default function App() {
  return (
    <ThemeProvider defaultTheme="dark" storageKey="vite-ui-theme">
      <Toaster richColors />
      <CookieDrawer />
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/klassementen" element={<Klassementen />} />
        <Route path="/uitslagen" element={<Uitslagen />} />
        <Route path="/clubs" element={<Clubs />} />
        <Route path="/wedstrijd/:id" element={<Wedstrijd />} />
        <Route element={<UnauthenticatedRoute />}>
          <Route path="/wedstrijd/:id/inschrijven" element={<Inschrijving />} />
        </Route>
        <Route path="/wedstrijd/:id/results" element={<Results />} />
        <Route path="/wedstrijd/:id/all-results" element={<AllResults />} />
        <Route path="/reset-wachtwoord" element={<ResetPassword />} />
        <Route path="/privacy-policy" element={<PrivacyPolicy />} />
        <Route path="/algemene-voorwaarden" element={<GeneralConditions />} />
        <Route path="/auth" element={<Auth />}>
          <Route path="/auth/login" element={<AuthLoginForm />} />
          <Route path="/auth/register" element={<AuthRegisterForm />} />
          <Route path="/auth/reset-wachtwoord" element={<ResetPassword />} />
          <Route path="/auth/confirm" element={<ConfirmEmail />} />
          <Route
            path="/auth/request-wachtwoord"
            element={<RequestPassword />}
          />
        </Route>
        <Route element={<ProtectedRoute requiredRole="Admin" />}>
          <Route path="/admin" element={<Admin />}>
            <Route index element={<AdminDashboard />} />
            <Route path="dashboard" element={<AdminDashboard />} />
            <Route path="data" element={<Data />}>
              <Route path="wedstrijden" element={<Wedstrijden />} />
              <Route path="personen" element={<Personen />} />
            </Route>
            <Route path="activity" element={<Dashboard />} />
            <Route path="instellingen" element={<Settings />}>
              <Route index element={<Profile />} />
              <Route path="account" element={<Account />} />
              <Route path="verwijder-account" element={<DeleteAccount />} />
              <Route path="profile" element={<Profile />} />
            </Route>
          </Route>
        </Route>
        <Route element={<ProtectedRoute requiredRole="User" />}>
          <Route path="/account" element={<AccountPage />}>
            <Route index element={<Dashboard />} />
            <Route path="dashboard" element={<Dashboard />} />
            <Route path="instellingen" element={<Settings />}>
              <Route index element={<Profile />} />
              <Route path="account" element={<Account />} />
              <Route path="verwijder-account" element={<DeleteAccount />} />
              <Route path="profile" element={<Profile />} />
            </Route>
          </Route>
        </Route>
        <Route path="*" element={<NotFound />} />
      </Routes>
    </ThemeProvider>
  );
}
