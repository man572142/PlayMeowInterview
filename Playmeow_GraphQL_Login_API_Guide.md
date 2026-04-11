# 📝 Playmeow GraphQL Login API Guide

This document describes how to implement login + auto-login using the Playmeow GraphQL backend.

---

## 1. Endpoint

**URL**
```
POST https://interview-api.join-playmeow.com/graphql
```

**Headers**
```
Content-Type: application/json
```

All GraphQL operations are sent to the same endpoint using HTTP POST.

---

## 2. Login Mutation

**Mutation name:** `login`

### Parameters

| Name | Type | Required | Description |
|------|------|----------|-------------|
| username | String | Yes | User email / username |
| password | String | Yes | User password |

### Example GraphQL Mutation

```graphql
mutation Login($username: String!, $password: String!) {
  login(username: $username, password: $password) {
    token
    user {
      id
      username
    }
  }
}
```

### Example HTTP Request Body (JSON)

```json
{
  "query": "mutation Login($username: String!, $password: String!) { login(username: $username, password: $password) { token user { id username } } }",
  "variables": {
    "username": "test@gmail.com",
    "password": "playmeow123"
  }
}
```

### Expected Response (Success)

```json
{
  "data": {
    "login": {
      "token": "JWT_TOKEN_HERE",
      "user": {
        "id": "123",
        "username": "test@gmail.com"
      }
    }
  }
}
```

### Error Handling Notes

GraphQL may return **HTTP 200 even when login fails**.

Example error response:

```json
{
  "data": null,
  "errors": [
    {
      "message": "Invalid credentials"
    }
  ]
}
```

Implementation should check:

- if `errors != null && errors.Length > 0` -> treat as failure
- if `data == null` -> treat as failure

---

## 3. Signup Mutation (Optional)

**Mutation name:** `signup`

### Parameters

| Name | Type | Required | Description |
|------|------|----------|-------------|
| username | String | Yes | User email / username |
| password | String | Yes | User password |

### Example GraphQL Mutation

```graphql
mutation Signup($username: String!, $password: String!) {
  signup(username: $username, password: $password) {
    token
    user {
      id
      username
    }
  }
}
```

---

## 4. AuthPayload Type

Both `login` and `signup` return the same type: `AuthPayload`.

### AuthPayload Fields

| Field | Type | Description |
|-------|------|-------------|
| token | String | JWT token used for authenticated requests |
| user | User | Logged-in user information |

---

## 5. Me Query (Auto-login / Token Validation)

**Query name:** `me`

This query is used to validate whether the stored token is still valid.

### Example GraphQL Query

```graphql
query {
  me {
    id
    username
  }
}
```

### Required Headers

After login, all authenticated requests should include:

```
Authorization: Bearer <token>
```

### Example HTTP Request Body (JSON)

```json
{
  "query": "{ me { id username } }"
}
```

### Expected Response (Success)

```json
{
  "data": {
    "me": {
      "id": "123",
      "username": "test@gmail.com"
    }
  }
}
```

If the token is invalid/expired, the server may return:

- `me: null`
- or GraphQL `errors`

Both cases should be treated as **not logged in**.

---

## 6. Unity Implementation Notes

### 6.1 Recommended Structure (Clean + Interview-Friendly)

- **GraphQLHttpClient**
  - Responsible for sending HTTP POST requests
  - Handles headers, serialization, timeout
  - Parses raw GraphQL response into a typed object

- **AuthService**
  - Provides `LoginAsync(username, password)`
  - Provides `GetMeAsync()`

- **TokenStore**
  - Saves token locally
  - Suggested implementation: `PlayerPrefsTokenStore`

This separation shows good architecture without overengineering.

---

## 7. UX Requirements

### 7.1 Prevent Double Click During Login

- Disable login button while request is in progress
- Re-enable only after request completes (success or failure)

### 7.2 Display Error Messages

- If GraphQL returns `errors[]`, show the first error message
- If parsing/network fails, show a generic error message

---

## 8. Auto-login Flow

On app start:

1. Load token from storage (`PlayerPrefs`)
2. If no token exists -> show Login scene
3. If token exists -> call `me` query with Authorization header
4. If `me` succeeds -> go to Main scene
5. If `me` fails -> clear token and show Login scene

---

## 9. Suggested Minimal Input Validation

- Email/username must not be empty
- Password must not be empty
- Optional: basic email format check
- Optional: Enter key triggers login
- Optional: password visibility toggle

---

## 10. Testing Account

```
username: test@gmail.com
password: playmeow123
```

---

## 11. Debugging Tips

- Log the raw JSON response if login fails
- GraphQL errors often contain useful server messages
- Always check `errors[]` even when HTTP request succeeds

---

## 12. Summary

To complete login implementation:

- Use `login(username, password)` mutation to obtain `token`
- Store token locally
- Use `Authorization: Bearer <token>` for future requests
- Use `me` query to validate token for auto-login
- Handle GraphQL errors correctly (HTTP 200 can still be failure)
