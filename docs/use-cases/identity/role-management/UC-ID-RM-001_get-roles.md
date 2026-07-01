## UC-001 - Get Roles

**Bounded Context:** Identity

**Actor:** Super Administrator

**Goal**
View a paginated list of roles with filtering options.

**Preconditions**

* User is authenticated.
* User has the `identity.roles.view` permission.

**Flow**

1. User requests the role list.
2. User may apply filters such as name, display name, is system role, is active, created from/to, and updated from/to.
3. System retrieves the matching roles.
4. System displays the roles as a paginated list.

**Result**

* A paginated list of roles is displayed.

**Requirements**

* FR-XXX