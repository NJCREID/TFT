const BASE_API_URL = import.meta.env.VITE_BASE_API_URL;

interface fetchRequestProps {
  endpoint: string;
  method?: "GET" | "POST" | "PATCH" | "PUT";
  body?: any;
  identifier?: string | null;
  authToken?: string;
}

export default async function fetchRequest<T>({
  endpoint,
  method = "GET",
  body,
  identifier,
  authToken,
}: fetchRequestProps): Promise<T> {
  if (endpoint.includes("{identifier}") && !identifier && identifier !== "") {
    throw new Error("Key is required for this endpoint.");
  }

  if (identifier || identifier === "") {
    endpoint = endpoint.replace("{identifier}", identifier);
  }

  const url = new URL(BASE_API_URL + endpoint);

  const requestOptions: RequestInit = {
    method: method,
    headers: {
      "Content-Type": "application/json",
      ...(authToken && { Authorization: `Bearer ${authToken}` }),
    },
    body: body ? JSON.stringify(body) : undefined,
  };

  return fetch(url.toString(), requestOptions)
    .then(async (response) => {
      const contentType = response.headers.get("content-type");

      if (!response.ok) {
        if (contentType && contentType.includes("application/json")) {
          return response.json().then((errorData) => {
            throw new Error(errorData.message || "An error occurred while fetching.");
          });
        } else {
          return response.text().then((errorText) => {
            throw new Error(errorText || "An error occured while fetching.");
          });
        }
      }
      return response.json() as Promise<T>;
    })
    .catch((error) => {
      console.error("Fetch request failed:", {
        message: error.message,
        stack: error.stack,
        url: url.toString(),
        method,
        body,
      });
      throw new Error(error.message || "An error occured while fetching.");
    });
}
