import nameLookup from "../data/names.json";

export interface NameInfo {
  name: string;
  imageLink: string;
}

export const lookupName = (key: string): { name: string; imageUrl: string } => {
  const entry = (nameLookup as { [key: string]: { name: string; type: string } })[key];
  if (entry) {
    const imageUrl = `https://localhost:7235/api/image/${entry.type}/${key}`;
    return { name: entry.name, imageUrl };
  }
  return { name: "Not Found", imageUrl: "Error" };
};
