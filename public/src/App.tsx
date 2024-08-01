import { Route, BrowserRouter, Routes } from "react-router-dom";
import { WindowDimensionsProvider } from "./context";
import { MainLayout } from "./layouts";
import {
  AugmentsStats,
  ChampionsStats,
  CommunityComps,
  CompsStats,
  Home,
  ItemsStats,
  MyTeamComps,
  TeamBuilder,
  TopComps,
  TraitsStats,
} from "./pages";
import "./App.css";
import { AuthProvider } from "./context/authContext";
import Login from "./pages/Login/Login";
import Register from "./pages/Register/Register";
import AuthLayout from "./layouts/AuthLayout/AuthLayout";
import { Provider } from "react-redux";
import store from "./store";
import MyPage from "./pages/MyPage/MyPage";
import ViewGuide from "./pages/ViewGuide/ViewGuide";

function AppRouter() {
  return (
    <Routes>
      <Route path="/" element={<AuthLayout />}>
        <Route path="/sign-in" element={<Login />} />
        <Route path="/register" element={<Register />} />
      </Route>
      <Route path="/" element={<MainLayout />}>
        <Route index element={<Home />} />
        <Route path="/auto-generated" element={<TopComps />} />
        <Route path="/comps-stats" element={<CompsStats />} />
        <Route path="/community-comps" element={<CommunityComps />} />
        <Route path="/my-team-comps" element={<MyTeamComps />} />
        <Route path="/team-builder" element={<TeamBuilder />} />
        <Route path="/champions-stats" element={<ChampionsStats />} />
        <Route path="/augments-stats" element={<AugmentsStats />} />
        <Route path="/items-stats" element={<ItemsStats />} />
        <Route path="/traits-stats" element={<TraitsStats />} />
        <Route path="/my-page" element={<MyPage />} />
        <Route path="/guide/:guideId" element={<ViewGuide />} />
      </Route>
    </Routes>
  );
}

function App() {
  return (
    <Provider store={store}>
      <BrowserRouter>
        <AuthProvider>
          <WindowDimensionsProvider>
            <AppRouter />
          </WindowDimensionsProvider>
        </AuthProvider>
      </BrowserRouter>
    </Provider>
  );
}

export default App;
