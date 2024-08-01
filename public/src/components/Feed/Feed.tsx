import { UserGuide } from "../../common/api";
import "./feed.css";
import TeamItem from "../TeamItem/TeamItem";

interface FeedProps {
  posts: UserGuide[];
}

const Feed = ({ posts }: FeedProps) => {
  return (
    <div className="app__feed">
      <div className="app__feed-post">
        {posts.map((userGuide, index) => (
          <TeamItem key={index} team={userGuide} />
        ))}
      </div>
    </div>
  );
};
export default Feed;
