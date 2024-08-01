// redux/modalSlice.ts
import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { Item, Unit, Augment } from "../common/api";

interface ModalState {
  modalContent: Item | Unit | Augment | null;
  modalPosition: { x: number; y: number; width: number; height: number } | null;
}

const initialState: ModalState = {
  modalContent: null,
  modalPosition: null,
};

const modalSlice = createSlice({
  name: "modal",
  initialState,
  reducers: {
    setModalContent: (
      state,
      action: PayloadAction<{
        content: Item | Unit | Augment | null;
        position: { x: number; y: number; width: number; height: number } | null;
      }>
    ) => {
      state.modalContent = action.payload.content;
      state.modalPosition = action.payload.position;
    },
    clearModalContent: (state) => {
      state.modalContent = null;
      state.modalPosition = null;
    },
  },
});

export const { setModalContent, clearModalContent } = modalSlice.actions;
export default modalSlice.reducer;
