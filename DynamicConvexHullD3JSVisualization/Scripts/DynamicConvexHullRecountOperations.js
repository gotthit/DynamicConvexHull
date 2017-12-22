'use strict';

var PointOnHullPosition = {
    OnTopOfHull: "On top of hull",
    BeforeTopOfHull: "Before top of hull", 
    AfterTopOfHull: "After top of hull"
}

function push_sub_hull_down(current_node) {
    push_determined_sub_hull_down(current_node, true);
    push_determined_sub_hull_down(current_node, false);
}

function push_determined_sub_hull_down(current_node, is_left) {
    if (current_node != null && !current_node.is_leaf() && current_node.get_convex_hull(is_left) != null) {

        let split_result = split_treap_by_index(current_node.get_convex_hull(is_left), current_node.get_lower_part_size(is_left));

        current_node.left_son.set_convex_hull(
            is_left, 
            merge_treaps(
                split_result.left_treap, 
                current_node.left_son.get_convex_hull(is_left)
            )
        );
        current_node.right_son.set_convex_hull(
            is_left, 
            merge_treaps(
                current_node.right_son.get_convex_hull(is_left),
                split_result.right_treap
            )
        );
        current_node.set_convex_hull(is_left, null);
    }
}

function recount_to_up(current_node) {
    if (current_node != null && !current_node.is_root()) {
        recount(current_node);
        current_node = current_node.parent;
    }
    recount(current_node);
    return current_node;
}

function recount(current_node) {
    recount_determined_hull(current_node, true);
    recount_determined_hull(current_node, false);
}

let current_lower_top;
let current_upper_top
let to_cut_from_lower_hull;
let to_leave_in_upper_hull;

let bridge_found;

function recount_determined_hull(current_node, is_left) {
    if (current_node != null && 
        current_node.left_son != null && 
        current_node.right_son != null && 
        current_node.get_convex_hull(is_left) == null) {
    
        current_node.max_point = current_node.left_son.max_point;
        if (current_node.max_point.compare_to(current_node.right_son.max_point) < 0) {
            current_node.max_point = current_node.right_son.max_point;
        }

        current_lower_top = current_node.left_son.get_convex_hull(is_left);
        current_upper_top = current_node.right_son.get_convex_hull(is_left);

        let medium_y = (current_lower_top.max_point.y + current_upper_top.min_point.y) / 2;

        to_cut_from_lower_hull = get_treap_size(current_lower_top.left_treap) + 1;
        to_leave_in_upper_hull = get_treap_size(current_upper_top.left_treap);

        bridge_found = false;
        while (!bridge_found) {

            var current_lower_top_point = current_lower_top.key;
            var current_upper_top_point = current_upper_top.key;

            let previous_lower_point = null;
            if (current_lower_top != null && current_lower_top.left_treap != null) {
                previous_lower_point = current_lower_top.left_treap.max_point;
            }
            let next_lower_point = null;
            if (current_lower_top != null && current_lower_top.right_treap != null) {
                previous_lower_point = current_lower_top.right_treap.min_point;
            }

            let previous_upper_point = null;
            if (current_upper_top != null && current_upper_top.left_treap != null) {
                previous_upper_point = current_upper_top.left_treap.max_point;
            }
            let next_upper_point = null;
            if (current_upper_top != null && current_upper_top.right_treap != null) {
                previous_upper_point = current_upper_top.right_treap.min_point;
            }

            _make_decision_about_next_operation(
                current_lower_top_point,
                previous_lower_point,
                next_lower_point,
                current_upper_top_point,
                previous_upper_point,
                next_upper_point,
                medium_y,
                is_left
            );
        }
        
        if (current_lower_top_point.y == current_upper_top_point.y) {
            if (is_left) {
                if (current_lower_top_point.x > current_upper_top_point.x) {
                    --to_cut_from_lower_hull;
                } else {
                    ++to_leave_in_upper_hull;
                }
            } else {
                if (current_lower_top_point.x < current_upper_top_point.x) {
                    --to_cut_from_lower_hull;
                } else {
                    ++to_leave_in_upper_hull;
                }
            }
        }

        concat_sub_hulls_to_current(current_node, to_cut_from_lower_hull, to_leave_in_upper_hull, is_left);
    }
}

function concat_sub_hulls_to_current(current_node, to_cut_from_lower_hull, to_leave_in_upper_hull, is_left) {

    let lower_split_result = split_treap_by_index(
        current_node.left_son.get_convex_hull(is_left), 
        to_cut_from_lower_hull
    );
    let upper_split_result = split_treap_by_index(
        current_node.right_son.get_convex_hull(is_left), 
        to_leave_in_upper_hull
    );

    current_node.set_lower_part_size(is_left, to_cut_from_lower_hull);
    current_node.set_convex_hull(
        is_left, 
        merge_treaps(
            lower_split_result.left_treap, 
            upper_split_result.right_treap
        )
    );
    current_node.left_son.set_convex_hull(is_left, lower_split_result.right_treap);
    current_node.right_son.set_convex_hull(is_left, upper_split_result.left_treap);
}

function _make_decision_about_next_operation(
    current_lower_top_point,
    previous_lower_point,
    next_lower_point,
    current_upper_top_point,
    previous_upper_point,
    next_upper_point,
    medium_y,
    is_left
) {
    let lower_top_point_posotion = determine_point_position_on_hull(
        current_lower_top_point, 
        current_upper_top_point, 
        previous_lower_point, 
        next_lower_point, 
        is_left
    );
    let upper_top_point_posotion = determine_point_position_on_hull(
        current_lower_top_point, 
        current_upper_top_point, 
        previous_upper_point, 
        next_upper_point, 
        is_left
    );

    if (lower_top_point_posotion == PointOnHullPosition.OnTopOfHull && 
        upper_top_point_posotion == PointOnHullPosition.OnTopOfHull) {             // A
        
        bridge_found = true;

    } else if (lower_top_point_posotion == PointOnHullPosition.AfterTopOfHull && 
        upper_top_point_posotion == PointOnHullPosition.OnTopOfHull) {             // B
        
        _go_left_in_lower_part();

    } else if (lower_top_point_posotion == PointOnHullPosition.BeforeTopOfHull && 
        upper_top_point_posotion == PointOnHullPosition.OnTopOfHull) {             // C
        
        _go_right_in_lower_part();

    } else if (lower_top_point_posotion == PointOnHullPosition.OnTopOfHull && 
        upper_top_point_posotion == PointOnHullPosition.BeforeTopOfHull) {         // D
        
        _go_right_in_upper_part();
        
    } else if (lower_top_point_posotion == PointOnHullPosition.OnTopOfHull && 
        upper_top_point_posotion == PointOnHullPosition.AfterTopOfHull) {          // E
        
        _go_left_in_upper_part();
        
    } else if (lower_top_point_posotion == PointOnHullPosition.AfterTopOfHull && 
        upper_top_point_posotion == PointOnHullPosition.BeforeTopOfHull) {         // F
        
        _go_left_in_lower_part();
        _go_right_in_upper_part();
        
    } else if (lower_top_point_posotion == PointOnHullPosition.AfterTopOfHull && 
        upper_top_point_posotion == PointOnHullPosition.AfterTopOfHull) {          // G
        
        _go_left_in_lower_part();
        
    } else if (lower_top_point_posotion == PointOnHullPosition.BeforeTopOfHull && 
        upper_top_point_posotion == PointOnHullPosition.BeforeTopOfHull) {         // H
        
        _go_right_in_upper_part();
        
    } else if (lower_top_point_posotion == PointOnHullPosition.BeforeTopOfHull && 
        upper_top_point_posotion == PointOnHullPosition.AfterTopOfHull) {         // I
        
        if (is_intersection_of_tangents_lower_then_y(
            current_lower_top_point, 
            next_lower_point, 
            current_upper_top_point, 
            previous_upper_point,
            medium_y,
            is_left
        )) {

            _go_right_in_lower_part();

        } else {

            _go_left_in_upper_part();

        }  
    }
}

function _go_left_in_lower_part() {
    current_lower_top = current_lower_top.left_treap;
    to_cut_from_lower_hull -= get_treap_size(current_lower_top.right_treap) + 1;
}

function _go_right_in_lower_part() {
    current_lower_top = current_lower_top.right_treap;
    to_cut_from_lower_hull += get_treap_size(current_lower_top.left_treap) + 1;
}

function _go_left_in_upper_part() {
    current_upper_top = current_upper_top.left_treap;
    to_leave_in_upper_hull -= get_treap_size(current_upper_top.right_treap) + 1;
}

function _go_right_in_upper_part() {
    current_upper_top = current_upper_top.right_treap;
    to_leave_in_upper_hull += get_treap_size(current_upper_top.left_treap) + 1;
}

function get_x_coord_of_line_by_y(first_point, second_point, y) {
    return (y - first_point.y) * (second_point.x - first_point.x) / (second_point.y - first_point.y) + first_point.x;
}

function is_intersection_of_tangents_lower_then_y(first_lower, second_lower, first_upper, second_upper, y, is_left) {
    let x_of_lower_line = get_x_coord_of_line_by_y(first_lower, second_lower);
    let x_of_upper_line = get_x_coord_of_line_by_y(first_upper, second_upper);
    if (is_left) {
        return x_of_lower_line < x_of_upper_line;
    } else {
        return x_of_lower_line > x_of_upper_line;
    }
}

function determine_point_position_on_hull(lower_top_point, upper_top_point, previous_point, next_point, is_left) {

    let previous_point_position = null;
    if (previous_point != null) {
        determine_posotion(lower_top_point, upper_top_point, previous_point);
    }
    if (previous_point_position == null || previous_point_position == PointPosition.On) {
        previous_point_position = is_left ? PointPosition.Right : PointPosition.Left;
    }

    let next_point_position = null;
    if (next_point != null) {
        determine_posotion(lower_top_point, upper_top_point, next_point);
    }
    if (next_point_position == null || next_point_position == PointPosition.On) {
        next_point_position = is_left ? PointPosition.Right : PointPosition.Left;
    }

    if (is_left) {
        if (previous_point_position == PointPosition.Right && next_point_position == PointPosition.Right) {
            return PointOnHullPosition.OnTopOfHull;
        }
        if (previous_point_position == PointPosition.Right && next_point_position == PointPosition.Left) {
            return PointOnHullPosition.AfterTopOfHull;
        }
        if (previous_point_position == PointPosition.Left && next_point_position == PointPosition.Right) {
            return PointOnHullPosition.BeforeTopOfHull;
        }
    } else {
        if (previous_point_position == PointPosition.Left && next_point_position == PointPosition.Left) {
            return PointOnHullPosition.OnTopOfHull;
        }
        if (previous_point_position == PointPosition.Left && next_point_position == PointPosition.Right) {
            return PointOnHullPosition.AfterTopOfHull;
        }
        if (previous_point_position == PointPosition.Right && next_point_position == PointPosition.Left) {
            return PointOnHullPosition.BeforeTopOfHull;
        }
    }
    // other variants are impossible
}