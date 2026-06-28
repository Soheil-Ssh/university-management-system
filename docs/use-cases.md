# UMS - Use Cases

## UC-001 - Create Role

**Bounded Context:** Identity

**Actor:** Super Administrator

**Goal**
Create a new role.

**Preconditions**

- User is authenticated.
- User has the `CreateRole` permission.

**Flow**

1. Enter the role name and an optional description.
2. System validates the input.
3. System ensures the role name is unique.
4. System creates the role.

**Result**
- The role is created successfully.

**Business Rules**
- Role names must be unique.

**Requirements**

* FR-XXX

## UC-002 - Update Role

**Bounded Context:** Identity

**Actor:** Super Administrator

**Goal**
Update an existing role.

**Preconditions**

- User is authenticated.
- User has the `UpdateRole` permission.

**Flow**

1. Select an existing role by role id.
2. Update the role name and/or description.
3. System validates the input.
4. System ensures that no other role has the same name.
5. System ensures that the role is not a system role.
6. System updates the role.

**Result**

* The role is updated successfully.

**Business Rules**

* Role names must be unique.
* System roles cannot be updated.

**Requirements**

* FR-XXX

## UC-003 - Get Role by ID

**Bounded Context:** Identity

**Actor:** Super Administrator

**Goal**
View the details of a specific role.

**Preconditions**

* User is authenticated.
* User has the `ViewRole` permission.

**Flow**

1. Select an existing role by its ID.
2. System retrieves the role information.
3. System displays the role details.

**Result**

* The role information is displayed.

**Requirements**

* FR-XXX
