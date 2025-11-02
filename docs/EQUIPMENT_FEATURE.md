# Equipment Entity - Feature Documentation

## Overview
The Equipment entity has been added to the SportZone backend to manage sports equipment inventory, rental, and sales.

## Database Schema

### Equipment Table
The `equipment` collection in MongoDB stores all equipment-related information with the following structure:

```
equipment {
  _id: ObjectId (Primary Key)
  name: string (required)
  type: EquipmentType enum (required)
  category: EquipmentCategory enum
  description: string
  imageUrl: string
  brand: string
  model: string
  sku: string
  
  // Inventory
  quantity: int (default: 1)
  condition: EquipmentCondition enum
  
  // Pricing
  purchasePrice: decimal
  rentalPricePerDay: decimal
  currency: string (default: 'USD')
  
  // Ownership and Location
  ownerId: ObjectId → users._id
  locationId: ObjectId
  
  // Availability
  availableForRent: boolean (default: false)
  availableForSale: boolean (default: false)
  isActive: boolean (default: true)
  
  // Maintenance
  maintenanceDate: timestamp
  purchaseDate: timestamp
  
  // Metadata
  tags: array of strings
  specifications: key-value pairs
  
  createdAt: timestamp
  updatedAt: timestamp
}
```

### Enumerations

#### EquipmentType
- Ball
- Racket
- Bat
- Glove
- Shoes
- Clothing
- ProtectiveGear
- Net
- Goal
- Mat
- Weights
- CardioEquipment
- TrainingAid
- Accessories
- Other

#### EquipmentCategory
- Sports
- Fitness
- Training
- Safety
- Accessories
- Maintenance
- Other

#### EquipmentCondition
- New
- Excellent
- Good
- Fair
- Poor
- NeedsRepair

### Relationships
- **equipment.ownerId → users._id**: Each equipment item belongs to a user

### Indexes
The following indexes are created for optimal query performance:
- `type`: For filtering equipment by type
- `ownerId`: For retrieving equipment owned by specific users
- `locationId`: For location-based queries
- `availableForRent`: For finding rentable equipment
- `availableForSale`: For finding equipment for sale

## API Endpoints

### Authentication
All endpoints require JWT Bearer token authentication except where noted.

### Endpoints

#### Create Equipment
```http
POST /api/equipment
Authorization: Bearer {token}
Content-Type: application/json

{
  "name": "Basketball",
  "type": "Ball",
  "category": "Sports",
  "description": "Official size basketball",
  "brand": "Spalding",
  "quantity": 5,
  "condition": "New",
  "purchasePrice": 29.99,
  "rentalPricePerDay": 5.00,
  "currency": "USD",
  "availableForRent": true,
  "availableForSale": false
}
```

#### Get Equipment by ID
```http
GET /api/equipment/{id}
Authorization: Bearer {token}
```

#### Get All Equipment
```http
GET /api/equipment
Authorization: Bearer {token}
```

#### Get My Equipment
```http
GET /api/equipment/my-equipment
Authorization: Bearer {token}
```

#### Get Equipment by Owner
```http
GET /api/equipment/owner/{ownerId}
Authorization: Bearer {token}
```

#### Get Equipment by Type
```http
GET /api/equipment/type/{type}
Authorization: Bearer {token}
```

#### Get Equipment by Location
```http
GET /api/equipment/location/{locationId}
Authorization: Bearer {token}
```

#### Get Available for Rent
```http
GET /api/equipment/available-for-rent
Authorization: Bearer {token}
```

#### Get Available for Sale
```http
GET /api/equipment/available-for-sale
Authorization: Bearer {token}
```

#### Update Equipment
```http
PUT /api/equipment/{id}
Authorization: Bearer {token}
Content-Type: application/json

{
  "quantity": 3,
  "condition": "Good",
  "availableForRent": false
}
```

#### Delete Equipment (Soft Delete)
```http
DELETE /api/equipment/{id}
Authorization: Bearer {token}
```

## DTOs

### CreateEquipmentDto
Used when creating new equipment. All validation rules are enforced.

### UpdateEquipmentDto
Used for updating equipment. All fields are optional - only provided fields will be updated.

### EquipmentResponseDto
Returned by all GET endpoints. Contains all equipment information.

## Business Rules

1. **Ownership**: Users can only update or delete equipment they own
2. **Soft Delete**: Equipment is never physically deleted, only marked as inactive
3. **Automatic Timestamps**: `createdAt` and `updatedAt` are managed automatically
4. **Owner Assignment**: The authenticated user is automatically set as the owner when creating equipment

## Testing

Three unit tests have been implemented in `SportZone.Tests/Repositories/EquipmentRepositoryTests.cs`:

1. **CreateAsync_ShouldInsertEquipmentAndReturnIt**: Verifies equipment creation
2. **GetByIdAsync_WhenEquipmentExists_ShouldReturnEquipment**: Tests successful retrieval
3. **GetByIdAsync_WhenEquipmentDoesNotExist_ShouldReturnNull**: Tests null handling

Run tests with:
```bash
dotnet test
```

## Implementation Files

- **Model**: `SportZone/Models/Equipment.cs`
- **Repository Interface**: `SportZone/Repositories/IEquipmentRepository.cs`
- **Repository**: `SportZone/Repositories/EquipmentRepository.cs`
- **Controller**: `SportZone/Controllers/EquipmentController.cs`
- **DTOs**: `SportZone/DTOs/EquipmentDtos.cs`
- **Tests**: `SportZone.Tests/Repositories/EquipmentRepositoryTests.cs`
- **Database Schema**: `docs/database-schema-with-equipment.dbml`

## Future Enhancements

Potential future features:
- Equipment rental booking system
- Equipment maintenance tracking and alerts
- Equipment usage statistics
- Equipment reservation system
- Integration with sport activities (link equipment to specific activities)
- Equipment marketplace features
- Equipment condition history tracking
- Automated depreciation calculations

## Notes

- All monetary values use decimal type for precision
- Equipment can be both for rent AND for sale simultaneously
- Tags and specifications provide flexible metadata storage
- The locationId field can reference physical locations or facilities (to be implemented)
