import "./feed.css";
import GuideItem from "../GuideItem/GuideItem";
import { UserGuide } from "../../data";

interface FeedProps {
  posts: UserGuide[];
}

const Feed = ({ posts }: FeedProps) => {
  return (
    <div className="app__feed">
      <div className="app__feed-post">
        {posts.map((userGuide, index) => (
          <GuideItem key={index} guide={userGuide} />
        ))}
      </div>
    </div>
  );
};
export default Feed;
