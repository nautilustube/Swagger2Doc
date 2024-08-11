# Swagger Petstore
This is a sample server Petstore server.  You can find out more about Swagger at [http://swagger.io](http://swagger.io) or on [irc.freenode.net, #swagger](http://swagger.io/irc/).  For this sample, you can use the api key `special-key` to test the authorization filters.
##  SecurityScheme

<br><br>

### - api_key(SecurityScheme)
| Name | Type | Description | In |
|------|------|-------------|----|
| ( * ) api_key | ApiKey |  | Header |
### - petstore_auth(SecurityScheme)
| Name | Type | Description | In |
|------|------|-------------|----|
| ( * )  | OAuth2 |  | Query |

<br><br>

## 1. **[POST]** /pet/{petId}/uploadImage
- Todo: uploads an image
### - Request Parameter Url:
| Name | Type | Description |
|------|------|-------------|
| ( * ) petId | integer int64   | ID of pet to update |
- petId Request Sample:
```json
   1
```
### - Request Parameter Form:
| Name | Type | Description |
|------|------|-------------|
|  additionalMetadata | string     | Additional data to pass to server |
|  file | string binary    | file to upload |
- multipart/form-data Request Sample JSON:
```json
   {
     "additionalMetadata": "string",
     "file": "string"
   }
```
### - Response Parameter Form:
| Code | Description |
|------|-------------|
| 200 | successful operation |
### - 回應参数表格:
| Name | Type | Description |
|------|------|-------------|
|  code | integer int32   |  |
|  type | string    |  |
|  message | string    |  |
- application/json Response Sample JSON:
```json
   {
     "code": 1,
     "type": "string",
     "message": "string"
   }
```

<br><br>

## 2. **[PUT]** /pet
- Todo: Update an existing pet
### - Request Parameter Url:
| Name | Type | Description |
|------|------|-------------|
### - Request Parameter Form:
| Name | Type | Description |
|------|------|-------------|
|  id | integer int64    |  |
|  category | object   Category  |  |
| ( * ) name | string     |  |
| ( * ) photoUrls | array     |  |
|  tags | array    <Tag> |  |
|  status | string     | pet status in the store |
- application/json Request Sample JSON:
```json
   {
     "id": 1,
     "category": {
       "id": 1,
       "name": "string"
     },
     "name": "Microsoft.OpenApi.Any.OpenApiString",
     "photoUrls": [
       "string"
     ],
     "tags": [
       {
         "id": 1,
         "name": "string"
       }
     ],
     "status": "string"
   }
```
### - Response Parameter Form:
| Code | Description |
|------|-------------|
| 400 | Invalid ID supplied |
### - Response Parameter Form:
| Code | Description |
|------|-------------|
| 404 | Pet not found |
### - Response Parameter Form:
| Code | Description |
|------|-------------|
| 405 | Validation exception |
## 2. **[POST]** /pet
- Todo: Add a new pet to the store
### - Request Parameter Url:
| Name | Type | Description |
|------|------|-------------|
### - Request Parameter Form:
| Name | Type | Description |
|------|------|-------------|
|  id | integer int64    |  |
|  category | object   Category  |  |
| ( * ) name | string     |  |
| ( * ) photoUrls | array     |  |
|  tags | array    <Tag> |  |
|  status | string     | pet status in the store |
- application/json Request Sample JSON:
```json
   {
     "id": 1,
     "category": {
       "id": 1,
       "name": "string"
     },
     "name": "Microsoft.OpenApi.Any.OpenApiString",
     "photoUrls": [
       "string"
     ],
     "tags": [
       {
         "id": 1,
         "name": "string"
       }
     ],
     "status": "string"
   }
```
### - Response Parameter Form:
| Code | Description |
|------|-------------|
| 405 | Invalid input |

<br><br>

## 3. **[GET]** /pet/findByStatus
- Todo: Finds Pets by status
### - Request Parameter Url:
| Name | Type | Description |
|------|------|-------------|
| ( * ) status | array    | Status values that need to be considered for filter |
- status Request Sample:
```json
   [
     "string"
   ]
```
### - Response Parameter Form:
| Code | Description |
|------|-------------|
| 200 | successful operation |
### - 回應参数表格:
| Name | Type | Description |
|------|------|-------------|
- application/json Response Sample JSON:
```json
   [
     {
       "id": 1,
       "category": {
         "id": 1,
         "name": "string"
       },
       "name": "Microsoft.OpenApi.Any.OpenApiString",
       "photoUrls": [
         "string"
       ],
       "tags": [
         {
           "id": 1,
           "name": "string"
         }
       ],
       "status": "string"
     }
   ]
```
### - Response Parameter Form:
| Code | Description |
|------|-------------|
| 400 | Invalid status value |

<br><br>

## 4. **[GET]** /pet/findByTags
- Todo: Finds Pets by tags
### - Request Parameter Url:
| Name | Type | Description |
|------|------|-------------|
| ( * ) tags | array    | Tags to filter by |
- tags Request Sample:
```json
   [
     "string"
   ]
```
### - Response Parameter Form:
| Code | Description |
|------|-------------|
| 200 | successful operation |
### - 回應参数表格:
| Name | Type | Description |
|------|------|-------------|
- application/json Response Sample JSON:
```json
   [
     {
       "id": 1,
       "category": {
         "id": 1,
         "name": "string"
       },
       "name": "Microsoft.OpenApi.Any.OpenApiString",
       "photoUrls": [
         "string"
       ],
       "tags": [
         {
           "id": 1,
           "name": "string"
         }
       ],
       "status": "string"
     }
   ]
```
### - Response Parameter Form:
| Code | Description |
|------|-------------|
| 400 | Invalid tag value |

<br><br>

## 5. **[GET]** /pet/{petId}
- Todo: Find pet by ID
### - Request Parameter Url:
| Name | Type | Description |
|------|------|-------------|
| ( * ) petId | integer int64   | ID of pet to return |
- petId Request Sample:
```json
   1
```
### - Response Parameter Form:
| Code | Description |
|------|-------------|
| 200 | successful operation |
### - 回應参数表格:
| Name | Type | Description |
|------|------|-------------|
|  id | integer int64   |  |
|  category | object  Category  |  |
| ( * ) name | string    |  |
| ( * ) photoUrls | array    |  |
|  tags | array   <Tag> |  |
|  status | string    | pet status in the store |
- application/json Response Sample JSON:
```json
   {
     "id": 1,
     "category": {
       "id": 1,
       "name": "string"
     },
     "name": "Microsoft.OpenApi.Any.OpenApiString",
     "photoUrls": [
       "string"
     ],
     "tags": [
       {
         "id": 1,
         "name": "string"
       }
     ],
     "status": "string"
   }
```
### - Response Parameter Form:
| Code | Description |
|------|-------------|
| 400 | Invalid ID supplied |
### - Response Parameter Form:
| Code | Description |
|------|-------------|
| 404 | Pet not found |
## 5. **[POST]** /pet/{petId}
- Todo: Updates a pet in the store with form data
### - Request Parameter Url:
| Name | Type | Description |
|------|------|-------------|
| ( * ) petId | integer int64   | ID of pet that needs to be updated |
- petId Request Sample:
```json
   1
```
### - Request Parameter Form:
| Name | Type | Description |
|------|------|-------------|
|  name | string     | Updated name of the pet |
|  status | string     | Updated status of the pet |
- application/x-www-form-urlencoded Request Sample JSON:
```json
   {
     "name": "string",
     "status": "string"
   }
```
### - Response Parameter Form:
| Code | Description |
|------|-------------|
| 405 | Invalid input |
## 5. **[DELETE]** /pet/{petId}
- Todo: Deletes a pet
### - Request Parameter Url:
| Name | Type | Description |
|------|------|-------------|
|  api_key | string    |  |
- api_key Request Sample:
```json
   "string"
```
| ( * ) petId | integer int64   | Pet id to delete |
- petId Request Sample:
```json
   1
```
### - Response Parameter Form:
| Code | Description |
|------|-------------|
| 400 | Invalid ID supplied |
### - Response Parameter Form:
| Code | Description |
|------|-------------|
| 404 | Pet not found |

<br><br>

## 6. **[GET]** /store/inventory
- Todo: Returns pet inventories by status
### - Request Parameter Url:
| Name | Type | Description |
|------|------|-------------|
### - Response Parameter Form:
| Code | Description |
|------|-------------|
| 200 | successful operation |
### - 回應参数表格:
| Name | Type | Description |
|------|------|-------------|
- application/json Response Sample JSON:
```json
   {}
```

<br><br>

## 7. **[POST]** /store/order
- Todo: Place an order for a pet
### - Request Parameter Url:
| Name | Type | Description |
|------|------|-------------|
### - Request Parameter Form:
| Name | Type | Description |
|------|------|-------------|
|  id | integer int64    |  |
|  petId | integer int64    |  |
|  quantity | integer int32    |  |
|  shipDate | string date-time    |  |
|  status | string     | Order Status |
|  complete | boolean     |  |
- application/json Request Sample JSON:
```json
   {
     "id": 1,
     "petId": 1,
     "quantity": 1,
     "shipDate": "string",
     "status": "string",
     "complete": true
   }
```
### - Response Parameter Form:
| Code | Description |
|------|-------------|
| 200 | successful operation |
### - 回應参数表格:
| Name | Type | Description |
|------|------|-------------|
|  id | integer int64   |  |
|  petId | integer int64   |  |
|  quantity | integer int32   |  |
|  shipDate | string date-time   |  |
|  status | string    | Order Status |
|  complete | boolean    |  |
- application/json Response Sample JSON:
```json
   {
     "id": 1,
     "petId": 1,
     "quantity": 1,
     "shipDate": "string",
     "status": "string",
     "complete": true
   }
```
### - Response Parameter Form:
| Code | Description |
|------|-------------|
| 400 | Invalid Order |

<br><br>

## 8. **[GET]** /store/order/{orderId}
- Todo: Find purchase order by ID
### - Request Parameter Url:
| Name | Type | Description |
|------|------|-------------|
| ( * ) orderId | integer int64   | ID of pet that needs to be fetched |
- orderId Request Sample:
```json
   1
```
### - Response Parameter Form:
| Code | Description |
|------|-------------|
| 200 | successful operation |
### - 回應参数表格:
| Name | Type | Description |
|------|------|-------------|
|  id | integer int64   |  |
|  petId | integer int64   |  |
|  quantity | integer int32   |  |
|  shipDate | string date-time   |  |
|  status | string    | Order Status |
|  complete | boolean    |  |
- application/json Response Sample JSON:
```json
   {
     "id": 1,
     "petId": 1,
     "quantity": 1,
     "shipDate": "string",
     "status": "string",
     "complete": true
   }
```
### - Response Parameter Form:
| Code | Description |
|------|-------------|
| 400 | Invalid ID supplied |
### - Response Parameter Form:
| Code | Description |
|------|-------------|
| 404 | Order not found |
## 8. **[DELETE]** /store/order/{orderId}
- Todo: Delete purchase order by ID
### - Request Parameter Url:
| Name | Type | Description |
|------|------|-------------|
| ( * ) orderId | integer int64   | ID of the order that needs to be deleted |
- orderId Request Sample:
```json
   1
```
### - Response Parameter Form:
| Code | Description |
|------|-------------|
| 400 | Invalid ID supplied |
### - Response Parameter Form:
| Code | Description |
|------|-------------|
| 404 | Order not found |

<br><br>

## 9. **[POST]** /user/createWithList
- Todo: Creates list of users with given input array
### - Request Parameter Url:
| Name | Type | Description |
|------|------|-------------|
### - Request Parameter Form:
| Name | Type | Description |
|------|------|-------------|
- application/json Request Sample JSON:
```json
   [
     {
       "id": 1,
       "username": "string",
       "firstName": "string",
       "lastName": "string",
       "email": "string",
       "password": "string",
       "phone": "string",
       "userStatus": 1
     }
   ]
```
### - Response Parameter Form:
| Code | Description |
|------|-------------|
| default | successful operation |

<br><br>

## 10. **[GET]** /user/{username}
- Todo: Get user by user name
### - Request Parameter Url:
| Name | Type | Description |
|------|------|-------------|
| ( * ) username | string    | The name that needs to be fetched. Use user1 for testing.  |
- username Request Sample:
```json
   "string"
```
### - Response Parameter Form:
| Code | Description |
|------|-------------|
| 200 | successful operation |
### - 回應参数表格:
| Name | Type | Description |
|------|------|-------------|
|  id | integer int64   |  |
|  username | string    |  |
|  firstName | string    |  |
|  lastName | string    |  |
|  email | string    |  |
|  password | string    |  |
|  phone | string    |  |
|  userStatus | integer int32   | User Status |
- application/json Response Sample JSON:
```json
   {
     "id": 1,
     "username": "string",
     "firstName": "string",
     "lastName": "string",
     "email": "string",
     "password": "string",
     "phone": "string",
     "userStatus": 1
   }
```
### - Response Parameter Form:
| Code | Description |
|------|-------------|
| 400 | Invalid username supplied |
### - Response Parameter Form:
| Code | Description |
|------|-------------|
| 404 | User not found |
## 10. **[PUT]** /user/{username}
- Todo: Updated user
### - Request Parameter Url:
| Name | Type | Description |
|------|------|-------------|
| ( * ) username | string    | name that need to be updated |
- username Request Sample:
```json
   "string"
```
### - Request Parameter Form:
| Name | Type | Description |
|------|------|-------------|
|  id | integer int64    |  |
|  username | string     |  |
|  firstName | string     |  |
|  lastName | string     |  |
|  email | string     |  |
|  password | string     |  |
|  phone | string     |  |
|  userStatus | integer int32    | User Status |
- application/json Request Sample JSON:
```json
   {
     "id": 1,
     "username": "string",
     "firstName": "string",
     "lastName": "string",
     "email": "string",
     "password": "string",
     "phone": "string",
     "userStatus": 1
   }
```
### - Response Parameter Form:
| Code | Description |
|------|-------------|
| 400 | Invalid user supplied |
### - Response Parameter Form:
| Code | Description |
|------|-------------|
| 404 | User not found |
## 10. **[DELETE]** /user/{username}
- Todo: Delete user
### - Request Parameter Url:
| Name | Type | Description |
|------|------|-------------|
| ( * ) username | string    | The name that needs to be deleted |
- username Request Sample:
```json
   "string"
```
### - Response Parameter Form:
| Code | Description |
|------|-------------|
| 400 | Invalid username supplied |
### - Response Parameter Form:
| Code | Description |
|------|-------------|
| 404 | User not found |

<br><br>

## 11. **[GET]** /user/login
- Todo: Logs user into the system
### - Request Parameter Url:
| Name | Type | Description |
|------|------|-------------|
| ( * ) username | string    | The user name for login |
- username Request Sample:
```json
   "string"
```
| ( * ) password | string    | The password for login in clear text |
- password Request Sample:
```json
   "string"
```
### - Response Parameter Form:
| Code | Description |
|------|-------------|
| 200 | successful operation |
### - 回應参数表格:
| Name | Type | Description |
|------|------|-------------|
- application/json Response Sample JSON:
```json
   "string"
```
### - Response Parameter Form:
| Code | Description |
|------|-------------|
| 400 | Invalid username/password supplied |

<br><br>

## 12. **[GET]** /user/logout
- Todo: Logs out current logged in user session
### - Request Parameter Url:
| Name | Type | Description |
|------|------|-------------|
### - Response Parameter Form:
| Code | Description |
|------|-------------|
| default | successful operation |

<br><br>

## 13. **[POST]** /user/createWithArray
- Todo: Creates list of users with given input array
### - Request Parameter Url:
| Name | Type | Description |
|------|------|-------------|
### - Request Parameter Form:
| Name | Type | Description |
|------|------|-------------|
- application/json Request Sample JSON:
```json
   [
     {
       "id": 1,
       "username": "string",
       "firstName": "string",
       "lastName": "string",
       "email": "string",
       "password": "string",
       "phone": "string",
       "userStatus": 1
     }
   ]
```
### - Response Parameter Form:
| Code | Description |
|------|-------------|
| default | successful operation |

<br><br>

## 14. **[POST]** /user
- Todo: Create user
### - Request Parameter Url:
| Name | Type | Description |
|------|------|-------------|
### - Request Parameter Form:
| Name | Type | Description |
|------|------|-------------|
|  id | integer int64    |  |
|  username | string     |  |
|  firstName | string     |  |
|  lastName | string     |  |
|  email | string     |  |
|  password | string     |  |
|  phone | string     |  |
|  userStatus | integer int32    | User Status |
- application/json Request Sample JSON:
```json
   {
     "id": 1,
     "username": "string",
     "firstName": "string",
     "lastName": "string",
     "email": "string",
     "password": "string",
     "phone": "string",
     "userStatus": 1
   }
```
### - Response Parameter Form:
| Code | Description |
|------|-------------|
| default | successful operation |

<br><br>

##  Schemas

<br><br>

### - ApiResponse
| Name | Type | Description |
|------|------|-------------|
|  code | integer int32   |  |
|  type | string    |  |
|  message | string    |  |
### - Category
| Name | Type | Description |
|------|------|-------------|
|  id | integer int64   |  |
|  name | string    |  |
### - Pet
| Name | Type | Description |
|------|------|-------------|
|  id | integer int64   |  |
|  category | object  Category  |  |
| ( * ) name | string    |  |
| ( * ) photoUrls | array    |  |
|  tags | array   <Tag> |  |
|  status | string    | pet status in the store |
### - Tag
| Name | Type | Description |
|------|------|-------------|
|  id | integer int64   |  |
|  name | string    |  |
### - Order
| Name | Type | Description |
|------|------|-------------|
|  id | integer int64   |  |
|  petId | integer int64   |  |
|  quantity | integer int32   |  |
|  shipDate | string date-time   |  |
|  status | string    | Order Status |
|  complete | boolean    |  |
### - User
| Name | Type | Description |
|------|------|-------------|
|  id | integer int64   |  |
|  username | string    |  |
|  firstName | string    |  |
|  lastName | string    |  |
|  email | string    |  |
|  password | string    |  |
|  phone | string    |  |
|  userStatus | integer int32   | User Status |

<br><br>

