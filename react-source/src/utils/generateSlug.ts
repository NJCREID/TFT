export function generateSlug(title: string): string {
  const slug: string = title.toLowerCase().replace(/ /g, "-");
  if (slug === "home") return "";
  if (slug === "settings") return "edit-profile";
  return slug;
}
