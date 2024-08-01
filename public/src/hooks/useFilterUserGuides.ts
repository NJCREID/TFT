import { useEffect, useState } from "react";
import fetchRequest, { UserGuide } from "../common/api";
import { ENDPOINT } from "../common/endpoints";
import { useAuthContext } from "../context/authContext";

export default function useFilterUserGuides() {
  const [searchQuery, setSearchQuery] = useState<string>("");
  const [traitFilter, setTraitFilter] = useState<string | null>(null);
  const [userGuides, setUserGuides] = useState<UserGuide[] | null>(null);
  const [filteredGuides, setFilteredGuides] = useState<UserGuide[] | null>(null);
  const { user } = useAuthContext();

  useEffect(() => {
    const fetchGuides = async () => {
      setUserGuides(
        await fetchRequest<UserGuide[]>(
          ENDPOINT.AUTO_GENERATED_GUIDE,
          "GET",
          null,
          user ? `?userId=${user.user.id}` : null
        )
      );
    };
    fetchGuides();
  }, [user]);

  useEffect(() => {
    if (!userGuides) return;

    const query = searchQuery.toLowerCase();

    let filteredUserGuides = userGuides.filter((userguide) => userguide.name.toLowerCase().includes(query));

    const hexFilteredUserGuides = userGuides.filter(
      (userguide) =>
        !filteredUserGuides.includes(userguide) &&
        userguide.hexes.some((hex) => hex.unit.name.toLowerCase().includes(query))
    );

    filteredUserGuides = [...filteredUserGuides, ...hexFilteredUserGuides];

    if (traitFilter) {
      filteredUserGuides = filteredUserGuides.filter((userguide) =>
        userguide.traits.some((trait) => trait.trait.key === traitFilter)
      );
    }
    setFilteredGuides(filteredUserGuides);
  }, [userGuides, searchQuery, traitFilter]);

  return {
    guides: filteredGuides,
    searchQuery,
    setSearchQuery,
    setTraitFilter,
  };
}
