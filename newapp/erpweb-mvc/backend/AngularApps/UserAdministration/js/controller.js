adminApp.controller('UserAdminController', [
    '$scope', '$http','$uibModal',
    function ($scope, $http, $uibModal) {
        $scope.Roles = model.Roles;
        $scope.Users = [];
        $scope.usersLoading = false;
        $scope.selected_role = null;
        $scope.NewUser = null;
        $scope.userSelected = false;
        $scope.UserList = [];
        $scope.tree_data = model.TreeNodes;
        $scope.message = '';
        $scope.col_defs = [
        {
            field: "",
            displayName: "",
            cellTemplate: '<button class="btn btn-warning" ng-click="cellTemplateScope.ManagePermissions(row.branch)" value="Manage permissions">Manage permissions</button>',
            cellTemplateScope: $scope        
        }];
        $scope.expand_on = {
            field: "text",
            displayName: "Item",
            sortable: true,
            filterable: true
        };
        $scope.filter = 1;
        $scope.filter_role = null;
        $scope.filter_user = null;

        $scope.permission_new_role = null;
        $scope.permission_new_user = null;
        $scope.permission_new_remove = false;
        $scope.navigation_item_id = null;
        $scope.NavigationItemPermissions = null;

        $scope.SelectRole = function (role) {
            $scope.selected_role = role;
            $scope.usersLoading = true;
            $http.post(usersInRolesUrl, { role_id: role.id }).then(function (retObj) {
                $scope.Users = retObj.data;
                $scope.usersLoading = false;
            });
        };

        $scope.GetUsers = function (text) {
            return $http.post(getUsersByTextUrl, { prefixText: text }).then(function (response) {
                $scope.UserList = response.data;
                return response.data;
            });
        };

        $scope.UserSelected = function ($item, $model, $label, $event) {
            $scope.NewUser = $item;
            $scope.userSelected = true;
        };

        $scope.AddUserToRole = function () {
            if (_.find($scope.Users, { user_id: $scope.NewUser.userid }))
                alert('User already assigned to role.');
            else {
                $http.post(addUserToRoleUrl, { user_id: $scope.NewUser.userid, role_id: $scope.selected_role.id }).then(function (response) {
                    $scope.Users.push(JSON.parse(JSON.stringify($scope.NewUser)));
                    $scope.NewUser = null;
                    $scope.userSelected = false;
                });
            }
        };

        $scope.RemoveUserFromRole = function (u) {
            $http.post(removeUserFromRoleUrl, { user_id: u.userid, role_id: $scope.selected_role.id }).then(function (response) {
                _.remove($scope.Users, u);
            });
        };

        $scope.ManagePermissions = function(branch) {
            $scope.navigation_item_id = branch.Item.id;
            $http.post(navigationItemPermissionsUrl, { id: branch.Item.id }).then(function(response) {
                $scope.NavigationItemPermissions = response.data;
                $scope.managePermissionsModal(response.data, $scope.Roles,
                    {
                        getUsers: $scope.GetUsers,
                        addPermissionForRole: $scope.AddPermissionForRole,
                        addPermissionForUser: $scope.AddPermissionForUser,
                        removePermission: $scope.RemovePermission,
                        filterRoles: $scope.FilterRoles,
                        filterUsers: $scope.FilterUsers
                    }
                 );
            });
        };

        $scope.managePermissionsModal = function (data, roles, functions) {
            var modalInstance = $uibModal.open({
                templateUrl: 'managePermissions.html',
                controller: ['$scope', '$uibModalInstance', 'data', 'roles','functions',
                    function ($scope, $uibModalInstance, data, roles, functions) {
                        $scope.NavigationItemPermissions = data;
                        $scope.Roles = roles;
                        $scope.GetUsers = functions.getUsers;
                        $scope.AddPermissionForRole = function () {
                            functions.addPermissionForRole($scope.permission_new_role).then(
                                function () {
                                    $scope.permission_new_role = null;
                                }
                            );
                        };
                        $scope.AddPermissionForUser = function () {
                            functions.addPermissionForUser($scope.permission_new_user.userid,
                                $scope.permission_new_remove | false).then(
                                    function () {
                                        $scope.permission_new_user = null;
                                        $scope.permission_new_remove = false;
                                    }
                                );
                        };
                        $scope.RemovePermission = functions.removePermission;
                        $scope.FilterRoles = functions.filterRoles;
                        $scope.FilterUsers = functions.filterUsers;
                        $scope.close = function () {
                            $uibModalInstance.dismiss();
                        };
                }],
                size: 'md',                
                resolve: {
                    data: function () {
                        return data;
                    },
                    roles: function () {
                        return roles;
                    },
                    functions: function () {
                        return functions;
                    }
                }
            });

            modalInstance.result.then(function () {
                
            }, function () {
                
            });
        }

        $scope.AddPermissionForRole = function (role_id) {
            if (_.find($scope.NavigationItemPermissions, { role_id: role_id }) == null) {
                return $http.post(addPermissionForRoleUrl, { item_id: $scope.navigation_item_id, role_id: role_id }).then(function(response) {
                    $scope.NavigationItemPermissions.push(response.data);                    
                });
            } else {
                $scope.message = 'The role already exists.';
                $('#modalAlert').modal();
            }
            
        };

        $scope.AddPermissionForUser = function (user_id, remove) {
            if (_.find($scope.NavigationItemPermissions, { user_id: user_id }) == null) {
                return $http.post(addPermissionForUserUrl, {
                    item_id: $scope.navigation_item_id,
                    user_id: user_id,
                    remove: remove
                }).then(function (response) {
                    $scope.NavigationItemPermissions.push(response.data);                    
                });
            } else {
                $scope.message = 'The user already exists.';
                $('#modalAlert').modal();
            }
        };

        $scope.RemovePermission = function(p) {
            $http.post(removePermissionUrl, { id: p.id }).then(function(response) {
                _.remove($scope.NavigationItemPermissions, { id: p.id });
            });
        };

        $scope.FilterRoles = function(elem) {
            return elem.role_id != null;
        };

        $scope.FilterUsers = function(elem) {
            return elem.user_id != null;
        };

        $scope.alertClose = function() {
            $('#modalAlert').modal('hide');
        };

        $scope.FilterItems = function() {
            var url;
            var data;
            var filter = parseInt($scope.filter);
            if (filter === 1) {
                url = navigationItemsUrl;
                data = {};
            }
            else if (filter == 2) {
                url = navigationItemsForRoleUrl;
                data = { role_id: $scope.filter_role };
            } else {
                url = navigationItemsForUserUrl;
                data = { user_id: $scope.filter_user.userid };
            }
            $http.get(url, { params: data }).then(function (response) {
                    $scope.tree_data = [];
                $scope.tree_data = response.data;
            },
                function(data) {
                    
                });

        };
    }
]);