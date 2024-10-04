import { useEffect, useState } from "react";
import fetchRequest from "../common/api";
import { useAuthContext } from "../context/authContext";
import { useDispatch } from "react-redux";
import { showError } from "../store/errorModalSlice";
import { UserGuide } from "../data";
import { sortTraits } from "../utils";

export default function useFilterUserGuides(endpoint: string) {
  const [searchQuery, setSearchQuery] = useState<string>("");
  const [traitFilter, setTraitFilter] = useState<string | null>(null);
  const [userGuides, setUserGuides] = useState<UserGuide[] | null>(null);
  const [filteredGuides, setFilteredGuides] = useState<UserGuide[] | null>(null);
  const { user } = useAuthContext();
  const dispatch = useDispatch();

  useEffect(() => {
    const fetchGuides = async () => {
      try {
        setUserGuides(null);
        let fetchedGuides = await fetchRequest<UserGuide[]>({
          endpoint,
          identifier: user ? `${user.user.id}` : "",
          authToken: user ? user.token : undefined,
        });
        setUserGuides(fetchedGuides);
      } catch (error) {
        setUserGuides([]);
        if (error instanceof Error) {
          dispatch(showError([`Error fetching guides: ${error.message}`]));
        } else {
          dispatch(showError(["An Unkown Error occured while fetching guides."]));
        }
      }
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
      filteredUserGuides = filteredUserGuides.filter((userguide) => {
        let traits = sortTraits(userguide.traits);
        return traits.some((trait) => trait.inGameKey === traitFilter);
      });
      console.log(filteredUserGuides);
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
