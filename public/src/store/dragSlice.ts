import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { Item, Unit, Augment } from "../common/api";

interface DragState {
  draggedItem: Unit | Item | Augment | null;
}

const initialState: DragState = {
  draggedItem: null,
};

const dragSlice = createSlice({
  name: "drag",
  initialState,
  reducers: {
    setDraggedItem: (state, action: PayloadAction<Unit | Item | Augment | null>) => {
      state.draggedItem = action.payload;
    },
    clearDraggedItem: (state) => {
      state.draggedItem = null;
    },
  },
});

export const { setDraggedItem, clearDraggedItem } = dragSlice.actions;
export default dragSlice.reducer;
