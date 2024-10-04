import { Augment, Item, Trait, Unit, UserGuide } from "../data";

export function isUnit(item: Unit | Item | Augment | Trait | null): item is Unit {
  return (item as Unit).cost !== undefined;
}

export function isItem(item: Unit | Item | Augment | Trait | null): item is Item {
  return (item as Item).recipe !== undefined;
}

export function isTrait(item: Unit | Item | Augment | Trait | null): item is Trait {
  return (item as Trait).tierString !== undefined;
}

export function isUserGuide(userGuide: UserGuide | null): userGuide is UserGuide {
  return (userGuide as UserGuide).userId !== undefined;
}
