# Swagger Petstore
This is a sample server Petstore server.  You can find out more about Swagger at [http://swagger.io](http://swagger.io) or on [irc.freenode.net, #swagger](http://swagger.io/irc/).  For this sample, you can use the api key `special-key` to test the authorization filters.
## 1. **[POST]** /pet/{petId}/uploadImage
### - 請求參數url:
- petId Request Sample:
```json
   1
```
### - 請求參數表格:
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
### - 回應参数表格:
| Code | Description |
|------|-------------|
| 200 | successful operation |
### - 回應参数表格:
| Name | Type | Description |
|------|------|-------------|
|  code | integer int32   |  |
|  type | string    |  |
|  message | string    |  |
- application/json 回應Sample JSON:
```json
   {
     "code": 1,
     "type": "string",
     "message": "string"
   }
```

<br><br>

## 2. **[PUT]** /pet
### - 請求參數表格:
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
### - 回應参数表格:
| Code | Description |
|------|-------------|
| 400 | Invalid ID supplied |
### - 回應参数表格:
| Code | Description |
|------|-------------|
| 404 | Pet not found |
### - 回應参数表格:
| Code | Description |
|------|-------------|
| 405 | Validation exception |
## 2. **[POST]** /pet
### - 請求參數表格:
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
### - 回應参数表格:
| Code | Description |
|------|-------------|
| 405 | Invalid input |

<br><br>

## 3. **[GET]** /pet/findByStatus
### - 請求參數url:
- status Request Sample:
```json
   [
     "string"
   ]
```
### - 回應参数表格:
| Code | Description |
|------|-------------|
| 200 | successful operation |
### - 回應参数表格:
| Name | Type | Description |
|------|------|-------------|
- application/json 回應Sample JSON:
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
### - 回應参数表格:
| Code | Description |
|------|-------------|
| 400 | Invalid status value |

<br><br>

## 4. **[GET]** /pet/findByTags
### - 請求參數url:
- tags Request Sample:
```json
   [
     "string"
   ]
```
### - 回應参数表格:
| Code | Description |
|------|-------------|
| 200 | successful operation |
### - 回應参数表格:
| Name | Type | Description |
|------|------|-------------|
- application/json 回應Sample JSON:
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
### - 回應参数表格:
| Code | Description |
|------|-------------|
| 400 | Invalid tag value |

<br><br>

## 5. **[GET]** /pet/{petId}
### - 請求參數url:
- petId Request Sample:
```json
   1
```
### - 回應参数表格:
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
- application/json 回應Sample JSON:
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
### - 回應参数表格:
| Code | Description |
|------|-------------|
| 400 | Invalid ID supplied |
### - 回應参数表格:
| Code | Description |
|------|-------------|
| 404 | Pet not found |
## 5. **[POST]** /pet/{petId}
### - 請求參數url:
- petId Request Sample:
```json
   1
```
### - 請求參數表格:
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
### - 回應参数表格:
| Code | Description |
|------|-------------|
| 405 | Invalid input |
## 5. **[DELETE]** /pet/{petId}
### - 請求參數url:
- api_key Request Sample:
```json
   "string"
```
### - 請求參數url:
- petId Request Sample:
```json
   1
```
### - 回應参数表格:
| Code | Description |
|------|-------------|
| 400 | Invalid ID supplied |
### - 回應参数表格:
| Code | Description |
|------|-------------|
| 404 | Pet not found |

<br><br>

## 6. **[GET]** /store/inventory
### - 回應参数表格:
| Code | Description |
|------|-------------|
| 200 | successful operation |
### - 回應参数表格:
| Name | Type | Description |
|------|------|-------------|
- application/json 回應Sample JSON:
```json
   {}
```

<br><br>

## 7. **[POST]** /store/order
### - 請求參數表格:
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
### - 回應参数表格:
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
- application/json 回應Sample JSON:
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
### - 回應参数表格:
| Code | Description |
|------|-------------|
| 400 | Invalid Order |

<br><br>

## 8. **[GET]** /store/order/{orderId}
### - 請求參數url:
- orderId Request Sample:
```json
   1
```
### - 回應参数表格:
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
- application/json 回應Sample JSON:
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
### - 回應参数表格:
| Code | Description |
|------|-------------|
| 400 | Invalid ID supplied |
### - 回應参数表格:
| Code | Description |
|------|-------------|
| 404 | Order not found |
## 8. **[DELETE]** /store/order/{orderId}
### - 請求參數url:
- orderId Request Sample:
```json
   1
```
### - 回應参数表格:
| Code | Description |
|------|-------------|
| 400 | Invalid ID supplied |
### - 回應参数表格:
| Code | Description |
|------|-------------|
| 404 | Order not found |

<br><br>

## 9. **[POST]** /user/createWithList
### - 請求參數表格:
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
### - 回應参数表格:
| Code | Description |
|------|-------------|
| default | successful operation |

<br><br>

## 10. **[GET]** /user/{username}
### - 請求參數url:
- username Request Sample:
```json
   "string"
```
### - 回應参数表格:
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
- application/json 回應Sample JSON:
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
### - 回應参数表格:
| Code | Description |
|------|-------------|
| 400 | Invalid username supplied |
### - 回應参数表格:
| Code | Description |
|------|-------------|
| 404 | User not found |
## 10. **[PUT]** /user/{username}
### - 請求參數url:
- username Request Sample:
```json
   "string"
```
### - 請求參數表格:
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
### - 回應参数表格:
| Code | Description |
|------|-------------|
| 400 | Invalid user supplied |
### - 回應参数表格:
| Code | Description |
|------|-------------|
| 404 | User not found |
## 10. **[DELETE]** /user/{username}
### - 請求參數url:
- username Request Sample:
```json
   "string"
```
### - 回應参数表格:
| Code | Description |
|------|-------------|
| 400 | Invalid username supplied |
### - 回應参数表格:
| Code | Description |
|------|-------------|
| 404 | User not found |

<br><br>

## 11. **[GET]** /user/login
### - 請求參數url:
- username Request Sample:
```json
   "string"
```
### - 請求參數url:
- password Request Sample:
```json
   "string"
```
### - 回應参数表格:
| Code | Description |
|------|-------------|
| 200 | successful operation |
### - 回應参数表格:
| Name | Type | Description |
|------|------|-------------|
- application/json 回應Sample JSON:
```json
   "string"
```
### - 回應参数表格:
| Code | Description |
|------|-------------|
| 400 | Invalid username/password supplied |

<br><br>

## 12. **[GET]** /user/logout
### - 回應参数表格:
| Code | Description |
|------|-------------|
| default | successful operation |

<br><br>

## 13. **[POST]** /user/createWithArray
### - 請求參數表格:
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
### - 回應参数表格:
| Code | Description |
|------|-------------|
| default | successful operation |

<br><br>

## 14. **[POST]** /user
### - 請求參數表格:
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
### - 回應参数表格:
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

