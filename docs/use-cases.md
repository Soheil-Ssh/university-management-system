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
- 