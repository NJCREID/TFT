export function GenerateSlug(title: string, name?: string): string {
  const slug: string = title.toLowerCase().replace(/ /g, "-");

  if (slug === "home") {
    return "";
  }

  if (name && name.toLowerCase() === "stats") {
    return `${slug}-stats`;
  }

  return slug;
}
