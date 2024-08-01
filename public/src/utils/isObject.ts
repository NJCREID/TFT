import { Augment, Item, Unit, UserGuide } from "../common/api";

export function isUnit(item: Unit | Item | Augment): item is Unit {
  return (item as Unit).cost !== undefined;
}

export function isItem(item: Unit | Item | Augment): item is Item {
  return (item as Item).recipe !== undefined;
}

export function isUserGuide(userGuide: UserGuide): userGuide is UserGuide {
  return userGuide.userId !== undefined;
}
