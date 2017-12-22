'use strict';

class DynamicConvexHull {

    constructor() {
        this.hull_root = null;
    }

    insert_point(point) {
        if (this.hull_root == null) {
            this.hull_root = new RedBlackNode(point);
        } else {
            this.hull_root = insert_point_to_hull(this.hull_root, point);
        }
    }

    delete_point(point) {
        if (this.hull_root != null) {
            if (this.hull_root.is_leaft() && this.hull_root.max_point.comparet_to(point) == null) {
                this.hull_root = null;
            } else {
                this.hull_root = delete_point_from_hull(this.hull_root, point);
            }
        }
    }

    get_hull_as_array() {
        if (this.hull_root == null) {
            return [];
        }

        let left_hull_array = this.hull_root.get_convex_hull(true).get_array(); 
        let right_hull_array = this.hull_root.get_convex_hull(false).get_array(); 
        right_hull_array = right_hull_array.reverse();

        let begin_right = 0;
        let end_right = right_hull_array.length;

        while (begin_right < left_hull_array.length &&
            begin_right < right_hull_array.length &&
            left_hull_array[left_hull_array.length - begin_right - 1].compare_to(right_hull_array[begin_right]) == 0) {

            ++begin_right;
        }
        while (right_hull_array.length - end_right < left_hull_array.length &&
            end_right >= 0 &&
            left_hull_array[right_hull_array.length - end_right].compare_to(right_hull_array[end_right - 1]) == 0) {

            --end_right;
        }

        if (end_right > begin_right) {
            left_hull_array = left_hull_array.concat(right_hull_array.slice(begin_right, end_right));
        }
        return left_hull_array;
    }
}