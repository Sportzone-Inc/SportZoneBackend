# Core Entities Schema Update

## Overview
This update refactors the core entities (User, Sport, SportActivity) to match the complete DBML schema design for the SportZone platform.

## Changes Made

### 1. **User Model** (`SportZone/Models/User.cs`)

#### New Fields Added:
- `ProfileImage` - URL to user profile image
- `Bio` - User biography/description
- `PhoneNumber` - Contact phone number
- `DateOfBirth` - User's date of birth
- `Gender` - Gender enum (Male, Female, Other, PreferNotToSay)
- `Latitude` / `Longitude` - Geolocation coordinates
- `SkillLevel` - Skill level enum (Beginner, Intermediate, Advanced, Expert)
- `IsActive` - Account active status (default: true)
- `IsVerified` - Email verification status (default: false)
- `LastLoginAt` - Last login timestamp

#### New Enums:
```csharp
public enum Gender
{
    Male, Female, Other, PreferNotToSay
}

public enum SkillLevel
{
    Beginner, Intermediate, Advanced, Expert
}
```

### 2. **Sport Model** (`SportZone/Models/Sport.cs`)

#### New Fields Added:
- `Category` - Sport category enum (Team, Individual, Water, Combat, etc.)
- `Description` - Detailed sport description
- `IconUrl` - URL to sport icon image
- `IsActive` - Sport active status (default: true)
- `UpdatedAt` - Last update timestamp

#### Enhanced SportType Enum:
```csharp
public enum SportType
{
    Basketball, Running, Football, Tennis, Soccer, Volleyball,
    Swimming, Cycling, Yoga, Golf, Baseball, Badminton,
    TableTennis, Climbing, Hiking, Skiing, Surfing, Boxing,
    MartialArts, Gym, Other
}
```

#### New Enum:
```csharp
public enum SportCategory
{
    Team, Individual, Water, Combat, Racquet, Fitness,
    Outdoor, Indoor, Extreme, Winter, Other
}
```

### 3. **SportActivity Model** (`SportZone/Models/SportActivity.cs`)

#### New Location Fields:
- `Address` - Street address
- `City` - City name
- `Country` - Country name

#### New Scheduling Fields:
- `StartTime` - Activity start time
- `EndTime` - Activity end time
- `Duration` - Duration in minutes

#### New Participant Fields:
- `MinParticipants` - Minimum participants required (default: 1)
- `Waitlist` - Array of user ObjectIds on waitlist

#### New Activity Details:
- `SkillLevelRequired` - Required skill level enum
- `AgeRestriction` - Age restriction string (e.g., "18+", "All ages")
- `CostPerPerson` - Cost per participant (default: 0)
- `Currency` - Currency code (default: "USD")
- `EquipmentProvided` - Whether equipment is provided (default: false)
- `EquipmentNeeded` - Description of needed equipment

#### New Status & Visibility Fields:
- `Status` - Activity status enum (Draft, Published, InProgress, Completed, Cancelled)
- `IsPublic` - Public visibility (default: true)
- `IsFeatured` - Featured status (default: false)

#### New Metadata:
- `OrganizerId` - Optional separate organizer ID
- `Views` - View count (default: 0)
- `Tags` - Array of tags
- `ImageUrls` - Array of image URLs

#### New Enums:
```csharp
public enum ActivitySkillLevel
{
    Any, Beginner, Intermediate, Advanced
}

public enum ActivityStatus
{
    Draft, Published, InProgress, Completed, Cancelled
}
```

## Database Indexes

### Users Collection
- `email` (unique)
- `username` (unique)
- `location`

### Sports Collection
- `type`
- `category`

### Sport Activities Collection
- `sportType`
- `status`
- `scheduledDate`
- `location`
- `(latitude, longitude)` - Geospatial index

## MongoDB Configuration

All models use MongoDB BSON attributes:
- `[BsonId]` - Primary key
- `[BsonRepresentation(BsonType.ObjectId)]` - ObjectId representation
- `[BsonElement("fieldName")]` - Field name mapping
- `[BsonRequired]` - Required field validation
- `[BsonRepresentation(BsonType.String)]` - Enum string representation

## Next Steps

Future iterations will add:
1. Social features (follows, posts, comments)
2. Activity interactions (participants with full details, invitations)
3. Messaging system (conversations, messages)
4. Notifications
5. Reviews & ratings
6. User preferences & settings
7. Achievements & gamification
8. Reports & moderation
9. Analytics & tracking

## Testing

Unit tests need to be created/updated for:
- New User fields and enums
- New Sport fields and enums
- New SportActivity fields and enums
- Validation logic for new fields

## Documentation

- Complete DBML schema: `docs/DATABASE_SCHEMA.dbml`
- Visual diagram: Generate at https://dbdiagram.io/

## Migration Notes

### For Existing Data:
When deploying, consider:
1. All new fields are nullable or have default values
2. No breaking changes to existing fields
3. Enums use string representation for flexibility
4. Existing data will work with default values

### Recommended Migration:
```csharp
// Example: Set default values for existing users
await _users.UpdateManyAsync(
    Builders<User>.Filter.Where(u => u.IsActive == null),
    Builders<User>.Update.Set(u => u.IsActive, true)
);
```

## Related PRs

This PR is part of the core schema update. Related PRs will follow for:
- Repository layer updates
- Service layer updates
- Controller updates with new DTOs
- Validation rules
- Unit tests
- Integration tests
