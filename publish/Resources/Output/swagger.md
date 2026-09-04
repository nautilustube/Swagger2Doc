# Swagger Petstore

版本 1.0.7｜產生日期 2026-09-04

This is a sample server Petstore server. You can find out more about Swagger at [http://swagger.io](http://swagger.io) or on [irc.freenode.net, #swagger](http://swagger.io/irc/). For this sample, you can use the api key `special-key` to test the authorization filters.

- 聯絡窗口： apiteam@swagger.io
- 授權：Apache 2.0

## 服務位址

| 位址 | 說明 |
|------|------|
| https://petstore.swagger.io/v2 |  |
| http://petstore.swagger.io/v2 |  |

## 安全性驗證

| 名稱 | 參數 | 類型 | 位置 | 說明 |
|------|------|------|------|------|
| api_key | api_key | ApiKey | Header |  |
| petstore_auth |  | OAuth2 | Query |  |

## API 目錄

| # | 方法 | 路徑 | 說明 |
|---|------|------|------|
| 1 | POST | [/pet/{petId}/uploadImage](#api-1) | uploads an image |
| 2 | PUT | [/pet](#api-2) | Update an existing pet |
| 3 | POST | [/pet](#api-3) | Add a new pet to the store |
| 4 | GET | [/pet/findByStatus](#api-4) | Finds Pets by status |
| 5 | GET | [/pet/findByTags](#api-5) | Finds Pets by tags |
| 6 | GET | [/pet/{petId}](#api-6) | Find pet by ID |
| 7 | POST | [/pet/{petId}](#api-7) | Updates a pet in the store with form data |
| 8 | DELETE | [/pet/{petId}](#api-8) | Deletes a pet |
| 9 | GET | [/store/inventory](#api-9) | Returns pet inventories by status |
| 10 | POST | [/store/order](#api-10) | Place an order for a pet |
| 11 | GET | [/store/order/{orderId}](#api-11) | Find purchase order by ID |
| 12 | DELETE | [/store/order/{orderId}](#api-12) | Delete purchase order by ID |
| 13 | POST | [/user/createWithList](#api-13) | Creates list of users with given input array |
| 14 | GET | [/user/{username}](#api-14) | Get user by user name |
| 15 | PUT | [/user/{username}](#api-15) | Updated user |
| 16 | DELETE | [/user/{username}](#api-16) | Delete user |
| 17 | GET | [/user/login](#api-17) | Logs user into the system |
| 18 | GET | [/user/logout](#api-18) | Logs out current logged in user session |
| 19 | POST | [/user/createWithArray](#api-19) | Creates list of users with given input array |
| 20 | POST | [/user](#api-20) | Create user |

---

## API 明細

### <a id="api-1"></a>1. [POST] /pet/{petId}/uploadImage

- 說明：uploads an image
- 分類：pet
- 代號：`uploadFile`

#### 請求參數（Path / Query / Header）

| 參數 | 位置 | 類型 | 必填 | 說明 |
|------|------|------|------|------|
| petId | Path | integer(int64) | 是 | ID of pet to update |

#### 請求內容（multipart/form-data）

| 欄位 | 類型 | 必填 | 說明 |
|------|------|------|------|
| additionalMetadata | string |  | Additional data to pass to server |
| file | string(binary) |  | file to upload |

請求範例：

```json
{
  "additionalMetadata": "string",
  "file": "string"
}
```

#### 回應狀態

| 狀態碼 | 說明 | 內容型別 |
|--------|------|----------|
| 200 | successful operation | application/json |

#### 回應內容 200（application/json）

| 欄位 | 類型 | 必填 | 說明 |
|------|------|------|------|
| code | integer(int32) |  |  |
| type | string |  |  |
| message | string |  |  |

回應範例 200：

```json
{
  "code": 0,
  "type": "string",
  "message": "string"
}
```

---

### <a id="api-2"></a>2. [PUT] /pet

- 說明：Update an existing pet
- 分類：pet
- 代號：`updatePet`

#### 請求內容（application/json）

| 欄位 | 類型 | 必填 | 說明 |
|------|------|------|------|
| id | integer(int64) |  |  |
| category | Category |  |  |
| name | string | 是 |  |
| photoUrls | array<string> | 是 |  |
| tags | array<Tag> |  |  |
| status | string |  | pet status in the store 〔可選值：available｜pending｜sold〕 |

請求範例：

```json
{
  "id": 0,
  "category": {
    "id": 0,
    "name": "string"
  },
  "name": "doggie",
  "photoUrls": [
    "string"
  ],
  "tags": [
    {
      "id": 0,
      "name": "string"
    }
  ],
  "status": "available"
}
```

#### 回應狀態

| 狀態碼 | 說明 | 內容型別 |
|--------|------|----------|
| 400 | Invalid ID supplied | — |
| 404 | Pet not found | — |
| 405 | Validation exception | — |

---

### <a id="api-3"></a>3. [POST] /pet

- 說明：Add a new pet to the store
- 分類：pet
- 代號：`addPet`

#### 請求內容（application/json）

| 欄位 | 類型 | 必填 | 說明 |
|------|------|------|------|
| id | integer(int64) |  |  |
| category | Category |  |  |
| name | string | 是 |  |
| photoUrls | array<string> | 是 |  |
| tags | array<Tag> |  |  |
| status | string |  | pet status in the store 〔可選值：available｜pending｜sold〕 |

請求範例：

```json
{
  "id": 0,
  "category": {
    "id": 0,
    "name": "string"
  },
  "name": "doggie",
  "photoUrls": [
    "string"
  ],
  "tags": [
    {
      "id": 0,
      "name": "string"
    }
  ],
  "status": "available"
}
```

#### 回應狀態

| 狀態碼 | 說明 | 內容型別 |
|--------|------|----------|
| 405 | Invalid input | — |

---

### <a id="api-4"></a>4. [GET] /pet/findByStatus

- 說明：Finds Pets by status
- 分類：pet
- 代號：`findPetsByStatus`

Multiple status values can be provided with comma separated strings

#### 請求參數（Path / Query / Header）

| 參數 | 位置 | 類型 | 必填 | 說明 |
|------|------|------|------|------|
| status | Query | array<string> | 是 | Status values that need to be considered for filter |

#### 回應狀態

| 狀態碼 | 說明 | 內容型別 |
|--------|------|----------|
| 200 | successful operation | application/json、application/xml |
| 400 | Invalid status value | — |

#### 回應內容 200（application/json）

型別：`array<Pet>`

回應範例 200：

```json
[
  {
    "id": 0,
    "category": {
      "id": 0,
      "name": "string"
    },
    "name": "doggie",
    "photoUrls": [
      "string"
    ],
    "tags": [
      {
        "id": 0,
        "name": "string"
      }
    ],
    "status": "available"
  }
]
```

---

### <a id="api-5"></a>5. [GET] /pet/findByTags

- 說明：Finds Pets by tags
- 分類：pet
- 代號：`findPetsByTags`
- 狀態：已淘汰（deprecated）

Multiple tags can be provided with comma separated strings. Use tag1, tag2, tag3 for testing.

#### 請求參數（Path / Query / Header）

| 參數 | 位置 | 類型 | 必填 | 說明 |
|------|------|------|------|------|
| tags | Query | array<string> | 是 | Tags to filter by |

#### 回應狀態

| 狀態碼 | 說明 | 內容型別 |
|--------|------|----------|
| 200 | successful operation | application/json、application/xml |
| 400 | Invalid tag value | — |

#### 回應內容 200（application/json）

型別：`array<Pet>`

回應範例 200：

```json
[
  {
    "id": 0,
    "category": {
      "id": 0,
      "name": "string"
    },
    "name": "doggie",
    "photoUrls": [
      "string"
    ],
    "tags": [
      {
        "id": 0,
        "name": "string"
      }
    ],
    "status": "available"
  }
]
```

---

### <a id="api-6"></a>6. [GET] /pet/{petId}

- 說明：Find pet by ID
- 分類：pet
- 代號：`getPetById`

Returns a single pet

#### 請求參數（Path / Query / Header）

| 參數 | 位置 | 類型 | 必填 | 說明 |
|------|------|------|------|------|
| petId | Path | integer(int64) | 是 | ID of pet to return |

#### 回應狀態

| 狀態碼 | 說明 | 內容型別 |
|--------|------|----------|
| 200 | successful operation | application/json、application/xml |
| 400 | Invalid ID supplied | — |
| 404 | Pet not found | — |

#### 回應內容 200（application/json）

| 欄位 | 類型 | 必填 | 說明 |
|------|------|------|------|
| id | integer(int64) |  |  |
| category | Category |  |  |
| name | string | 是 |  |
| photoUrls | array<string> | 是 |  |
| tags | array<Tag> |  |  |
| status | string |  | pet status in the store 〔可選值：available｜pending｜sold〕 |

回應範例 200：

```json
{
  "id": 0,
  "category": {
    "id": 0,
    "name": "string"
  },
  "name": "doggie",
  "photoUrls": [
    "string"
  ],
  "tags": [
    {
      "id": 0,
      "name": "string"
    }
  ],
  "status": "available"
}
```

---

### <a id="api-7"></a>7. [POST] /pet/{petId}

- 說明：Updates a pet in the store with form data
- 分類：pet
- 代號：`updatePetWithForm`

#### 請求參數（Path / Query / Header）

| 參數 | 位置 | 類型 | 必填 | 說明 |
|------|------|------|------|------|
| petId | Path | integer(int64) | 是 | ID of pet that needs to be updated |

#### 請求內容（application/x-www-form-urlencoded）

| 欄位 | 類型 | 必填 | 說明 |
|------|------|------|------|
| name | string |  | Updated name of the pet |
| status | string |  | Updated status of the pet |

請求範例：

```json
{
  "name": "string",
  "status": "string"
}
```

#### 回應狀態

| 狀態碼 | 說明 | 內容型別 |
|--------|------|----------|
| 405 | Invalid input | — |

---

### <a id="api-8"></a>8. [DELETE] /pet/{petId}

- 說明：Deletes a pet
- 分類：pet
- 代號：`deletePet`

#### 請求參數（Path / Query / Header）

| 參數 | 位置 | 類型 | 必填 | 說明 |
|------|------|------|------|------|
| api_key | Header | string |  |  |
| petId | Path | integer(int64) | 是 | Pet id to delete |

#### 回應狀態

| 狀態碼 | 說明 | 內容型別 |
|--------|------|----------|
| 400 | Invalid ID supplied | — |
| 404 | Pet not found | — |

---

### <a id="api-9"></a>9. [GET] /store/inventory

- 說明：Returns pet inventories by status
- 分類：store
- 代號：`getInventory`

Returns a map of status codes to quantities

#### 回應狀態

| 狀態碼 | 說明 | 內容型別 |
|--------|------|----------|
| 200 | successful operation | application/json |

#### 回應內容 200（application/json）

型別：`object`

回應範例 200：

```json
{}
```

---

### <a id="api-10"></a>10. [POST] /store/order

- 說明：Place an order for a pet
- 分類：store
- 代號：`placeOrder`

#### 請求內容（application/json）

| 欄位 | 類型 | 必填 | 說明 |
|------|------|------|------|
| id | integer(int64) |  |  |
| petId | integer(int64) |  |  |
| quantity | integer(int32) |  |  |
| shipDate | string(date-time) |  |  |
| status | string |  | Order Status 〔可選值：placed｜approved｜delivered〕 |
| complete | boolean |  |  |

請求範例：

```json
{
  "id": 0,
  "petId": 0,
  "quantity": 0,
  "shipDate": "2024-01-01T00:00:00",
  "status": "placed",
  "complete": true
}
```

#### 回應狀態

| 狀態碼 | 說明 | 內容型別 |
|--------|------|----------|
| 200 | successful operation | application/json、application/xml |
| 400 | Invalid Order | — |

#### 回應內容 200（application/json）

| 欄位 | 類型 | 必填 | 說明 |
|------|------|------|------|
| id | integer(int64) |  |  |
| petId | integer(int64) |  |  |
| quantity | integer(int32) |  |  |
| shipDate | string(date-time) |  |  |
| status | string |  | Order Status 〔可選值：placed｜approved｜delivered〕 |
| complete | boolean |  |  |

回應範例 200：

```json
{
  "id": 0,
  "petId": 0,
  "quantity": 0,
  "shipDate": "2024-01-01T00:00:00",
  "status": "placed",
  "complete": true
}
```

---

### <a id="api-11"></a>11. [GET] /store/order/{orderId}

- 說明：Find purchase order by ID
- 分類：store
- 代號：`getOrderById`

For valid response try integer IDs with value >= 1 and <= 10. Other values will generated exceptions

#### 請求參數（Path / Query / Header）

| 參數 | 位置 | 類型 | 必填 | 說明 |
|------|------|------|------|------|
| orderId | Path | integer(int64) | 是 | ID of pet that needs to be fetched 〔範圍 1 ~ 10〕 |

#### 回應狀態

| 狀態碼 | 說明 | 內容型別 |
|--------|------|----------|
| 200 | successful operation | application/json、application/xml |
| 400 | Invalid ID supplied | — |
| 404 | Order not found | — |

#### 回應內容 200（application/json）

| 欄位 | 類型 | 必填 | 說明 |
|------|------|------|------|
| id | integer(int64) |  |  |
| petId | integer(int64) |  |  |
| quantity | integer(int32) |  |  |
| shipDate | string(date-time) |  |  |
| status | string |  | Order Status 〔可選值：placed｜approved｜delivered〕 |
| complete | boolean |  |  |

回應範例 200：

```json
{
  "id": 0,
  "petId": 0,
  "quantity": 0,
  "shipDate": "2024-01-01T00:00:00",
  "status": "placed",
  "complete": true
}
```

---

### <a id="api-12"></a>12. [DELETE] /store/order/{orderId}

- 說明：Delete purchase order by ID
- 分類：store
- 代號：`deleteOrder`

For valid response try integer IDs with positive integer value. Negative or non-integer values will generate API errors

#### 請求參數（Path / Query / Header）

| 參數 | 位置 | 類型 | 必填 | 說明 |
|------|------|------|------|------|
| orderId | Path | integer(int64) | 是 | ID of the order that needs to be deleted 〔範圍 1 ~ -〕 |

#### 回應狀態

| 狀態碼 | 說明 | 內容型別 |
|--------|------|----------|
| 400 | Invalid ID supplied | — |
| 404 | Order not found | — |

---

### <a id="api-13"></a>13. [POST] /user/createWithList

- 說明：Creates list of users with given input array
- 分類：user
- 代號：`createUsersWithListInput`

#### 請求內容（application/json）

型別：`array<User>`

請求範例：

```json
[
  {
    "id": 0,
    "username": "string",
    "firstName": "string",
    "lastName": "string",
    "email": "string",
    "password": "string",
    "phone": "string",
    "userStatus": 0
  }
]
```

#### 回應狀態

| 狀態碼 | 說明 | 內容型別 |
|--------|------|----------|
| default | successful operation | — |

---

### <a id="api-14"></a>14. [GET] /user/{username}

- 說明：Get user by user name
- 分類：user
- 代號：`getUserByName`

#### 請求參數（Path / Query / Header）

| 參數 | 位置 | 類型 | 必填 | 說明 |
|------|------|------|------|------|
| username | Path | string | 是 | The name that needs to be fetched. Use user1 for testing. |

#### 回應狀態

| 狀態碼 | 說明 | 內容型別 |
|--------|------|----------|
| 200 | successful operation | application/json、application/xml |
| 400 | Invalid username supplied | — |
| 404 | User not found | — |

#### 回應內容 200（application/json）

| 欄位 | 類型 | 必填 | 說明 |
|------|------|------|------|
| id | integer(int64) |  |  |
| username | string |  |  |
| firstName | string |  |  |
| lastName | string |  |  |
| email | string |  |  |
| password | string |  |  |
| phone | string |  |  |
| userStatus | integer(int32) |  | User Status |

回應範例 200：

```json
{
  "id": 0,
  "username": "string",
  "firstName": "string",
  "lastName": "string",
  "email": "string",
  "password": "string",
  "phone": "string",
  "userStatus": 0
}
```

---

### <a id="api-15"></a>15. [PUT] /user/{username}

- 說明：Updated user
- 分類：user
- 代號：`updateUser`

This can only be done by the logged in user.

#### 請求參數（Path / Query / Header）

| 參數 | 位置 | 類型 | 必填 | 說明 |
|------|------|------|------|------|
| username | Path | string | 是 | name that need to be updated |

#### 請求內容（application/json）

| 欄位 | 類型 | 必填 | 說明 |
|------|------|------|------|
| id | integer(int64) |  |  |
| username | string |  |  |
| firstName | string |  |  |
| lastName | string |  |  |
| email | string |  |  |
| password | string |  |  |
| phone | string |  |  |
| userStatus | integer(int32) |  | User Status |

請求範例：

```json
{
  "id": 0,
  "username": "string",
  "firstName": "string",
  "lastName": "string",
  "email": "string",
  "password": "string",
  "phone": "string",
  "userStatus": 0
}
```

#### 回應狀態

| 狀態碼 | 說明 | 內容型別 |
|--------|------|----------|
| 400 | Invalid user supplied | — |
| 404 | User not found | — |

---

### <a id="api-16"></a>16. [DELETE] /user/{username}

- 說明：Delete user
- 分類：user
- 代號：`deleteUser`

This can only be done by the logged in user.

#### 請求參數（Path / Query / Header）

| 參數 | 位置 | 類型 | 必填 | 說明 |
|------|------|------|------|------|
| username | Path | string | 是 | The name that needs to be deleted |

#### 回應狀態

| 狀態碼 | 說明 | 內容型別 |
|--------|------|----------|
| 400 | Invalid username supplied | — |
| 404 | User not found | — |

---

### <a id="api-17"></a>17. [GET] /user/login

- 說明：Logs user into the system
- 分類：user
- 代號：`loginUser`

#### 請求參數（Path / Query / Header）

| 參數 | 位置 | 類型 | 必填 | 說明 |
|------|------|------|------|------|
| username | Query | string | 是 | The user name for login |
| password | Query | string | 是 | The password for login in clear text |

#### 回應狀態

| 狀態碼 | 說明 | 內容型別 |
|--------|------|----------|
| 200 | successful operation | application/json、application/xml |
| 400 | Invalid username/password supplied | — |

#### 回應內容 200（application/json）

型別：`string`

回應範例 200：

```json
"string"
```

---

### <a id="api-18"></a>18. [GET] /user/logout

- 說明：Logs out current logged in user session
- 分類：user
- 代號：`logoutUser`

#### 回應狀態

| 狀態碼 | 說明 | 內容型別 |
|--------|------|----------|
| default | successful operation | — |

---

### <a id="api-19"></a>19. [POST] /user/createWithArray

- 說明：Creates list of users with given input array
- 分類：user
- 代號：`createUsersWithArrayInput`

#### 請求內容（application/json）

型別：`array<User>`

請求範例：

```json
[
  {
    "id": 0,
    "username": "string",
    "firstName": "string",
    "lastName": "string",
    "email": "string",
    "password": "string",
    "phone": "string",
    "userStatus": 0
  }
]
```

#### 回應狀態

| 狀態碼 | 說明 | 內容型別 |
|--------|------|----------|
| default | successful operation | — |

---

### <a id="api-20"></a>20. [POST] /user

- 說明：Create user
- 分類：user
- 代號：`createUser`

This can only be done by the logged in user.

#### 請求內容（application/json）

| 欄位 | 類型 | 必填 | 說明 |
|------|------|------|------|
| id | integer(int64) |  |  |
| username | string |  |  |
| firstName | string |  |  |
| lastName | string |  |  |
| email | string |  |  |
| password | string |  |  |
| phone | string |  |  |
| userStatus | integer(int32) |  | User Status |

請求範例：

```json
{
  "id": 0,
  "username": "string",
  "firstName": "string",
  "lastName": "string",
  "email": "string",
  "password": "string",
  "phone": "string",
  "userStatus": 0
}
```

#### 回應狀態

| 狀態碼 | 說明 | 內容型別 |
|--------|------|----------|
| default | successful operation | — |

---

## 資料模型

| 模型 | 欄位數 |
|------|--------|
| [ApiResponse](#schema-ApiResponse) | 3 |
| [Category](#schema-Category) | 2 |
| [Order](#schema-Order) | 6 |
| [Pet](#schema-Pet) | 6 |
| [Tag](#schema-Tag) | 2 |
| [User](#schema-User) | 8 |

### <a id="schema-ApiResponse"></a>ApiResponse

| 欄位 | 類型 | 必填 | 說明 |
|------|------|------|------|
| code | integer(int32) |  |  |
| type | string |  |  |
| message | string |  |  |

### <a id="schema-Category"></a>Category

| 欄位 | 類型 | 必填 | 說明 |
|------|------|------|------|
| id | integer(int64) |  |  |
| name | string |  |  |

### <a id="schema-Order"></a>Order

| 欄位 | 類型 | 必填 | 說明 |
|------|------|------|------|
| id | integer(int64) |  |  |
| petId | integer(int64) |  |  |
| quantity | integer(int32) |  |  |
| shipDate | string(date-time) |  |  |
| status | string |  | Order Status 〔可選值：placed｜approved｜delivered〕 |
| complete | boolean |  |  |

### <a id="schema-Pet"></a>Pet

| 欄位 | 類型 | 必填 | 說明 |
|------|------|------|------|
| id | integer(int64) |  |  |
| category | Category |  |  |
| name | string | 是 |  |
| photoUrls | array<string> | 是 |  |
| tags | array<Tag> |  |  |
| status | string |  | pet status in the store 〔可選值：available｜pending｜sold〕 |

### <a id="schema-Tag"></a>Tag

| 欄位 | 類型 | 必填 | 說明 |
|------|------|------|------|
| id | integer(int64) |  |  |
| name | string |  |  |

### <a id="schema-User"></a>User

| 欄位 | 類型 | 必填 | 說明 |
|------|------|------|------|
| id | integer(int64) |  |  |
| username | string |  |  |
| firstName | string |  |  |
| lastName | string |  |  |
| email | string |  |  |
| password | string |  |  |
| phone | string |  |  |
| userStatus | integer(int32) |  | User Status |

