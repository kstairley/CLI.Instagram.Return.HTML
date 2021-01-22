Parser.Instagram.Return.HTML
====

## Purpose
This library contains functionality to part the HTML included in a Instagram return and store the data in a SqlLite database that is used by .Social.

## Using this repo
In order to include this utility in another project, please add this repo as a subtree of your main project's repo.  Once the subtree has been established, include the project in your solution and add references as appropriate.  Please update this document to add/remove the main project listing in the Referencing Projects section.

## IMPORTANT - THIS REPO CONTAINS AT LEAST ONE SUBTREE!
In order to work with the other repositories that this project is dependant on, please run the following command after cloning the project:

```
git remote add -f tools http://solver-gitlab.solver.net/tech_share/Utility.Tools.git
```

## Adding this utility as a subtree in your main project
1. In your main repo, define a new remote that points to this repo for quick reference using the following command:

```
git remote add -f fbrhtml http://solver-gitlab.solver.net/tech_share/Parser.Instagram.Return.HTML.git
```
2. Add the project as a subtree using the following command:

**_Example -_**

```
git subtree add --prefix <subfolder_name> <repo_name> <branch_name> --squash
```
In order to follow established naming conventions, the actual command to run should be:

**_Actual Command for this utility -_**

```
git subtree add --prefix Parser.Instagram.Return.HTML fbrhtml master --squash
```
*Note - "master" is the name of the branch you will pull into your main repo directory.  It can be replaced with another branch name or a specific commit sha if desired.

## Projects Referencing this Utility
* 

-----
## References
Please see https://artemis.atlassian.net/wiki/spaces/PD/pages/41648134/git+subtrees
for a great tutorial on using git subtrees.

