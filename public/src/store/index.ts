import { configureStore } from "@reduxjs/toolkit";
import modalReducer from "./modalSlice";
import dragReducer from "./dragSlice";
import errorModalSlice from "./errorModalSlice";

const store = configureStore({
  reducer: {
    modal: modalReducer,
    drag: dragReducer,
    errorModal: errorModalSlice,
  },
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
export default store;
