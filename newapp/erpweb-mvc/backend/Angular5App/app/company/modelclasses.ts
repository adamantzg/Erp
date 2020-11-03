import { User, Checkable } from "../common";

export class SelectableUser extends User implements Checkable {
    checked = false;
}