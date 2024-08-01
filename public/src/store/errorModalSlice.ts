import { createSlice, PayloadAction } from "@reduxjs/toolkit";

interface ErrorModalState {
  messages: string[] | null;
}

const initialState: ErrorModalState = {
  messages: null,
};

const errorModalSlice = createSlice({
  name: "errorModal",
  initialState,
  reducers: {
    showError(state, action: PayloadAction<string[]>) {
      state.messages = action.payload;
    },
    clearError(state) {
      state.messages = null;
    },
  },
});

export const { showError, clearError } = errorModalSlice.actions;
export default errorModalSlice.reducer;
