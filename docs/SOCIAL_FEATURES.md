# Social Features Implementation

## Overview
This update adds complete social networking features to the SportZone platform, including follows, posts, and comments with full CRUD API support.

## Changes Made

### 1. **Models Created**

#### Follow Model (`SportZone/Models/Follow.cs`)
- Represents user follow relationships
- Fields: `Id`, `FollowerId`, `FollowingId`, `CreatedAt`
- Supports follower/following relationships

#### Post Model (`SportZone/Models/Post.cs`)
- Represents user posts and updates
- Fields: `Id`, `UserId`, `ActivityId`, `Title`, `Body`, `PostType`, `MediaUrls`, `ThumbnailUrl`, `Likes`, `LikesCount`, `CommentsCount`, `SharesCount`, `Visibility`, `IsPinned`, `Tags`, `Location`, `Latitude`, `Longitude`, timestamps
- **Enums**:
  - `PostType`: Text, Image, Video, Activity, Achievement
  - `PostVisibility`: Public, Friends, Private

#### Comment Model (`SportZone/Models/Comment.cs`)
- Represents comments on posts
- Fields: `Id`, `PostId`, `UserId`, `ParentCommentId`, `Body`, `Likes`, `LikesCount`, `IsActive`, `IsEdited`, timestamps
- Supports nested replies via `ParentCommentId`

### 2. **Repositories Created**

#### FollowRepository
- **Interface**: `IFollowRepository`
- **Implementation**: `FollowRepository`
- **Methods**:
  - `CreateAsync` - Create a follow relationship
  - `GetByIdAsync` - Get follow by ID
  - `GetByFollowerAndFollowingAsync` - Check specific relationship
  - `GetFollowersByUserIdAsync` - Get all followers of a user
  - `GetFollowingByUserIdAsync` - Get all users a user is following
  - `GetFollowersCountAsync` - Count followers
  - `GetFollowingCountAsync` - Count following
  - `IsFollowingAsync` - Check if user A follows user B
  - `DeleteAsync` - Delete by ID
  - `DeleteByFollowerAndFollowingAsync` - Delete specific relationship

#### PostRepository
- **Interface**: `IPostRepository`
- **Implementation**: `PostRepository`
- **Methods**:
  - `CreateAsync` - Create a post
  - `GetByIdAsync` - Get post by ID
  - `GetAllAsync` - Get all posts
  - `GetByUserIdAsync` - Get posts by user
  - `GetByActivityIdAsync` - Get posts related to activity
  - `GetFeedForUserAsync` - Get paginated feed
  - `UpdateAsync` - Update a post
  - `DeleteAsync` - Soft delete a post
  - `LikeAsync` - Like a post
  - `UnlikeAsync` - Unlike a post
  - `IncrementCommentsCountAsync` - Increment comment counter
  - `DecrementCommentsCountAsync` - Decrement comment counter

#### CommentRepository
- **Interface**: `ICommentRepository`
- **Implementation**: `CommentRepository`
- **Methods**:
  - `CreateAsync` - Create a comment
  - `GetByIdAsync` - Get comment by ID
  - `GetByPostIdAsync` - Get comments for a post
  - `GetRepliesByCommentIdAsync` - Get replies to a comment
  - `UpdateAsync` - Update a comment
  - `DeleteAsync` - Soft delete a comment
  - `LikeAsync` - Like a comment
  - `UnlikeAsync` - Unlike a comment

### 3. **DTOs Created**

#### FollowDtos.cs
- `CreateFollowDto` - For creating follows
- `FollowResponseDto` - For returning follow data
- `FollowStatsDto` - For follower/following counts

#### PostDtos.cs
- `CreatePostDto` - For creating posts
- `UpdatePostDto` - For updating posts
- `PostResponseDto` - For returning post data

#### CommentDtos.cs
- `CreateCommentDto` - For creating comments
- `UpdateCommentDto` - For updating comments
- `CommentResponseDto` - For returning comment data

### 4. **Controllers with Full CRUD**

#### FollowsController (`/api/follows`)
**Endpoints**:
- `POST /api/follows` - Follow a user
- `DELETE /api/follows/{followingId}` - Unfollow a user
- `GET /api/follows/{id}` - Get specific follow relationship
- `GET /api/follows/user/{userId}/followers` - Get user's followers
- `GET /api/follows/user/{userId}/following` - Get users the user is following
- `GET /api/follows/user/{userId}/stats` - Get follower/following counts
- `GET /api/follows/is-following/{userId}` - Check if following a user

#### PostsController (`/api/posts`)
**Endpoints**:
- `POST /api/posts` - Create a post
- `GET /api/posts/{id}` - Get a post by ID
- `GET /api/posts` - Get all posts
- `GET /api/posts/user/{userId}` - Get posts by user
- `GET /api/posts/activity/{activityId}` - Get posts by activity
- `GET /api/posts/feed?skip=0&limit=20` - Get personalized feed (paginated)
- `PUT /api/posts/{id}` - Update a post
- `DELETE /api/posts/{id}` - Delete a post
- `POST /api/posts/{id}/like` - Like a post
- `DELETE /api/posts/{id}/like` - Unlike a post

#### CommentsController (`/api/comments`)
**Endpoints**:
- `POST /api/comments` - Create a comment
- `GET /api/comments/{id}` - Get a comment by ID
- `GET /api/comments/post/{postId}` - Get comments for a post
- `GET /api/comments/{commentId}/replies` - Get replies to a comment
- `PUT /api/comments/{id}` - Update a comment
- `DELETE /api/comments/{id}` - Delete a comment
- `POST /api/comments/{id}/like` - Like a comment
- `DELETE /api/comments/{id}/like` - Unlike a comment

### 5. **Database Indexes**

#### Follows Collection
- Compound unique index on `(followerId, followingId)`
- Index on `followerId`
- Index on `followingId`

#### Posts Collection
- Index on `userId`
- Index on `activityId`
- Descending index on `createdAt`

#### Comments Collection
- Index on `postId`
- Index on `userId`
- Index on `parentCommentId`

### 6. **Authentication & Authorization**

All social feature endpoints require JWT authentication via `[Authorize]` attribute:
- Users can only update/delete their own posts and comments
- Follow/unfollow requires authenticated user
- Proper ownership checks implemented

### 7. **Features Implemented**

#### Follow System
✅ Follow/unfollow users
✅ Get followers and following lists
✅ Get follower/following counts
✅ Check if user A follows user B
✅ Prevent self-following
✅ Prevent duplicate follows

#### Posts System
✅ Create text, image, video, activity, and achievement posts
✅ Edit and delete own posts
✅ Like/unlike posts with counter
✅ Comment counter maintenance
✅ Pin posts
✅ Public/friends/private visibility
✅ Tag posts with hashtags
✅ Attach location (with coordinates)
✅ Link posts to activities
✅ Paginated feed
✅ Filter by user or activity
✅ Soft delete (maintain data integrity)

#### Comments System
✅ Comment on posts
✅ Reply to comments (nested comments via `parentCommentId`)
✅ Edit and delete own comments
✅ Like/unlike comments with counter
✅ Auto-maintain post comment counters
✅ Edit tracking (`isEdited` flag)
✅ Soft delete

## API Usage Examples

### Follow a User
```bash
POST /api/follows
Authorization: Bearer {token}
{
  "followingId": "507f1f77bcf86cd799439011"
}
```

### Create a Post
```bash
POST /api/posts
Authorization: Bearer {token}
{
  "title": "Amazing run today!",
  "body": "Just completed my first 10K run",
  "postType": "Activity",
  "activityId": "507f1f77bcf86cd799439012",
  "tags": ["running", "fitness", "10k"],
  "visibility": "Public",
  "mediaUrls": ["https://example.com/image1.jpg"]
}
```

### Get User Feed
```bash
GET /api/posts/feed?skip=0&limit=20
Authorization: Bearer {token}
```

### Comment on a Post
```bash
POST /api/comments
Authorization: Bearer {token}
{
  "postId": "507f1f77bcf86cd799439013",
  "body": "Great job! Keep it up!"
}
```

### Reply to a Comment
```bash
POST /api/comments
Authorization: Bearer {token}
{
  "postId": "507f1f77bcf86cd799439013",
  "parentCommentId": "507f1f77bcf86cd799439014",
  "body": "Thanks!"
}
```

## Testing Notes

### Unit Tests Needed (To Be Created)
- Follow creation and validation
- Duplicate follow prevention
- Self-follow prevention
- Post CRUD operations
- Post like/unlike functionality
- Comment CRUD operations
- Comment threading (nested replies)
- Counter maintenance (likes, comments)
- Soft delete functionality
- Authorization checks

### Integration Tests Needed
- Complete user workflow: Register → Follow → Post → Comment → Like
- Feed generation and pagination
- Comment threading and nested replies
- Cross-model consistency (post comment counts)

## MongoDB Collections

### follows
```javascript
{
  _id: ObjectId,
  followerId: ObjectId,
  followingId: ObjectId,
  createdAt: ISODate
}
```

### posts
```javascript
{
  _id: ObjectId,
  userId: ObjectId,
  activityId: ObjectId (optional),
  title: String,
  body: String,
  postType: String (enum),
  mediaUrls: [String],
  thumbnailUrl: String,
  likes: [ObjectId],
  likesCount: Number,
  commentsCount: Number,
  sharesCount: Number,
  visibility: String (enum),
  isActive: Boolean,
  isPinned: Boolean,
  tags: [String],
  location: String,
  latitude: Number,
  longitude: Number,
  createdAt: ISODate,
  updatedAt: ISODate
}
```

### comments
```javascript
{
  _id: ObjectId,
  postId: ObjectId,
  userId: ObjectId,
  parentCommentId: ObjectId (optional),
  body: String,
  likes: [ObjectId],
  likesCount: Number,
  isActive: Boolean,
  isEdited: Boolean,
  createdAt: ISODate,
  updatedAt: ISODate
}
```

## Next Steps

1. Create NUnit tests for all repositories
2. Create NUnit tests for all controllers
3. Add user information enrichment (username, profile image) in responses
4. Implement actual personalized feed algorithm (currently shows public posts)
5. Add notification system for follows, likes, and comments
6. Add share functionality for posts
7. Add post reporting/moderation
8. Add image/video upload service integration
9. Add real-time updates via SignalR
10. Performance optimization with caching

## Related PRs

- Depends on: PR #7 (Core Entities Schema Update)
- Future: Notifications system
- Future: Activity participants and invitations
- Future: Messaging system

## Breaking Changes

None - This is a new feature addition with no impact on existing functionality.

## Deployment Notes

- Ensure MongoDB replica set is configured for transactions (if using transaction-based operations in the future)
- Indexes will be created automatically on first run
- No data migration needed
