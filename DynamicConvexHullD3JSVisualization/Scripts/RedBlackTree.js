'use strict';

var NodeColor = {
    Red: "Red",
    Black: "Black"
}

class RedBlackNode {

    constructor(point, parent = null) {
        this.parent = parent;
        this.left_son = null;
        this.right_son = null;

        this.max_point = point;
        this.node_color = this.is_root() ? NodeColor.Black : NodeColor.Red;

        this.left_convex_hull = new Treap(point);
        this.lower_part_size_of_left_hull = 0;

        this.right_convex_hull = new Treap(point);
        this.lower_part_size_of_right_hull = 0;
    }

    is_root() {
        return this.parent == null;
    }

    is_leaf() {
        return this.left_son == null && this.right_son == null;
    }

    grandparent() {
        if (this.parent == null) {
            return null;
        }
        return this.parent.parent;
    }

    uncle() {
        if (this.grandparent() == null) {
            return null;
        }
        if (this.parent == this.grandparent().left_son) {
            return this.grandparent().right_son;
        }
        return this.grandparent().left_son;
    }

    brother() {
        if (this.parent == null) {
            return null;
        }
        if (this.parent.left_son == this) {
            return this.parent.right_son;
        }
        return this.parent.left_son;
    }

    is_left_son() {
        if (this.parent != null) {
            return this.parent.left_son == this;
        }
        return false;
    }

    is_right_son() {
        if (this.parent != null) {
            return this.parent.right_son == this;
        }
        return false;
    }

    get_convex_hull(is_left) {
        return is_left ? this.left_convex_hull : this.right_convex_hull;
    }

    set_convex_hull(is_left, convex_hull) {
        if (is_left) {
            this.left_convex_hull = convex_hull;
        } else {
            this.right_convex_hull = convex_hull;
        }
    }

    get_lower_part_size(is_left) {
        return is_left ? this.lower_part_size_of_left_hull : this.lower_part_size_of_right_hull;
    }

    set_lower_part_size(is_left, size) {
        if (is_left) {
            this.lower_part_size_of_left_hull = size;
        } else {
            this.lower_part_size_of_right_hull = size;
        }
    }

    rotate_left() {
        if (this.right_son != null) {
            let future_parent = this.right_son;
            future_parent.parent = this.parent;
            if (this.parent != null) {
                if (this.is_left_son()) {
                    this.parent.left_son = future_parent;
                } else {
                    this.parent.right_son = future_parent;
                }
            }
            this.right_son = future_parent.left_son;
            if (future_parent.left_son != null) {
                future_parent.left_son.parent = this;
            }
            future_parent.left_son = this;
            this.parent = future_parent;
        }
    }

    rotate_right() {
        let future_parent = this.left_son;
        future_parent.parent = this.parent;
        if (this.parent != null) {
            if (this.is_left_son()) {
                this.parent.left_son = future_parent;
            } else {
                this.parent.right_son = future_parent;
            }
        }
        this.left_son = future_parent.right_son;
        if (future_parent.right_son != null) {
            future_parent.right_son.parent = this;
        }
        future_parent.right_son = this;
        this.parent = future_parent;
    }
}