## UC-ID-RM-002 - Get Role by ID

**Bounded Context:** Identity

**Actor:** Super Administrator

**Goal**
View the details of a specific role.

**Preconditions**

* User is authenticated.
* User has the `identity.roles.read` permission.

**Flow**

1. Select an existing role by its ID.
2. System retrieves the role information.
3. System displays the role details.

**Result**

* The role information is displayed.

**Requirements**

* FR-XXX