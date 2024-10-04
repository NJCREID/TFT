import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { Augment, Item, Trait, Unit } from "../data";

interface DragState {
  draggedItem: Unit | Item | Augment | Trait | null;
}

const initialState: DragState = {
  draggedItem: null,
};

const dragSlice = createSlice({
  name: "drag",
  initialState,
  reducers: {
    setDraggedItem: (state, action: PayloadAction<Unit | Item | Augment | Trait | null>) => {
      state.draggedItem = action.payload;
    },
    clearDraggedItem: (state) => {
      state.draggedItem = null;
    },
  },
});

export const { setDraggedItem, clearDraggedItem } = dragSlice.actions;
export default dragSlice.reducer;
