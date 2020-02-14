# API Request Examples

## Retrieve all users

```
curl --request GET 'http://localhost:9090/users/' --header 'Content-Type: application/json'
```

## Create a user

```
curl --location --request POST 'http://localhost:9090/users/' --header 'Content-Type: application/json' --data-raw '{"mailAddress":"obarrios@suse.com","lives":3, "wrongAnswers" : []}'
```

# Update a user
```
curl --location --request PUT 'http://localhost:9090/users/obarrios@suse.com' \
--header 'Content-Type: application/json' \
--data-raw '{"mailAddress":"obarrios@suse.com","lives":1, "wrongAnswers" : []}'
```
