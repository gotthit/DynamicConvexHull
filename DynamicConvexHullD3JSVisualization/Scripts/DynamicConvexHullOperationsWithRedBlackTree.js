'use strict';

function insert_point_to_hull(current_node, point_to_insert) {

    if (current_node.is_leaf() && point_to_insert.compare_to(current_node.max_point) != 0) {
        if (point_to_insert.compare_to(current_node.max_point) < 0) {
            current_node.left_son = new RedBlackNode(point_to_insert, current_node);
            current_node.right_son = new RedBlackNode(current_node.max_point, current_node);
        } else {
            current_node.left_son = new RedBlackNode(current_node.max_point, current_node);
            current_node.right_son = new RedBlackNode(point_to_insert, current_node);
        }
        current_node.set_convex_hull(true, null);
        current_node.set_convex_hull(false, null);

        return initial_repair_after_insert(current_node);
    } else if (!current_node.is_leaf()) {
        push_sub_hull_down(current_node);

        if (point_to_insert.compare_to(current_node.left_son.max_point) <= 0) {
            return insert_point_to_hull(current_node.left_son, point_to_insert);
        } else {
            return insert_point_to_hull(current_node.right_son, point_to_insert);
        }
    }
    return recount_to_up(current_node);
}

function initial_repair_after_insert(current_node) {
    // considering the specific of our insertion - if current not black (in this case we have to do nothing)
    // current and current.Brother were red leafs before last insert, so we can make start already from 
    // case 3 in https://ru.wikipedia.org/wiki/%D0%9A%D1%80%D0%B0%D1%81%D0%BD%D0%BE-%D1%87%D1%91%D1%80%D0%BD%D0%BE%D0%B5_%D0%B4%D0%B5%D1%80%D0%B5%D0%B2%D0%BE 

    if (current_node.node_color == NodeColor.Red) {
        return repair_after_insert_case_3(current_node.left_son);
    } else {
        return recount_to_up(current_node);
    }
} 

function repair_after_insert_cases_1_2(current_node) {
    recount(current_node);
    if (current_node.is_root()) {
        current_node.node_color = NodeColor.Black;
        return current_node;
    }
    if (current_node.parent.node_color == NodeColor.Red) {
        return repair_after_insert_case_3(current_node);
    }
    return recount_to_up(current_node);
}

function repair_after_insert_case_3(current_node) {
    if (current_node.uncle() != null && current_node.uncle().node_color == NodeColor.Red) {

        current_node.parent.node_color = NodeColor.Black;
        current_node.uncle().node_color = NodeColor.Black;
        current_node.grandparent().node_color = NodeColor.Red;

        recount(current_node.parent);
        return repair_after_insert_cases_1_2(current_node.grandparent());
    } else {
        return repair_after_insert_case_4(current_node)
    }
}

function repair_after_insert_case_4(current_node) {
    if (current_node.parent.is_left_son() && current_node.is_right_son()) {

        push_sub_hull_down(current_node);
        current_node.parent.rotate_left();
        current_node = current_node.left_son;

        recount(current_node);

    } else if (current_node.parent.is_right_son() && current_node.is_left_son()) {

        push_sub_hull_down(current_node);
        current_node.parent.rotate_right();
        current_node = current_node.right_son;

        recount(current_node);
    } 
    return repair_after_insert_case_5(current_node);
}

function repair_after_insert_case_5(current_node) {
    current_node.parent.node_color = NodeColor.Black;
    current_node.grandparent().node_color = NodeColor.Red;

    if (current_node.is_left_son()) {
        current_node.grandparent().rotate_right();
        current_node = current_node.parent;
        recount(current_node.right_son);
    } else {
        current_node.grandparent().rotate_left();
        current_node = current_node.parent;
        recount(current_node.left_son);
    }

    return recount_to_up(current_node);
}

function delete_point_from_hull(current_node, point_to_delete) {
    // we will delete only leafs
    // every node have two children

    if (current_node.is_leaf() && current_node.max_point.compare_to(point_to_delete) == 0) {
        if (current_node.grandparent() != null) {
            if (current_node.parent.is_left_son()) {
                current_node.grandparent().left_son = current_node.brother();
            } else {
                current_node.grandparent().right_son = current_node.brother();
            }
        }
        current_node.brother().parent = current_node.grandparent();

        if (current_node.brother().node_color == NodeColor.Red || current_node.parent.node_color == NodeColor.Red) {
            current_node.brother().node_color = NodeColor.Black;
            return recount_to_up(current_node.brother());
        } else {
            return repair_after_delete_case_1(current_node.brother());
        }
    } else if (!current_node.is_leaf()) {
        push_sub_hull_down(current_node);

        if (point_to_delete.compare_to(current_node.left_son.max_point) <= 0) {
            return delete_point_from_hull(current_node.left_son, point_to_delete);
        } else {
            return delete_point_from_hull(current_node.right_son, point_to_delete);
        }
    }
    return recount_to_up(current_node);
}

function repair_after_delete_case_1(current_node) {
    recount(current_node);
    if (current_node.is_root()) {
        return current_node;
    }
    return repair_after_delete_case_2(current_node);
}

function repair_after_delete_case_2(current_node) {
    if (current_node.brother().node_color == NodeColor.Red) {

        current_node.parent.node_color = NodeColor.Red;
        current_node.brother().node_color = NodeColor.Black;

        push_sub_hull_down(current_node.brother());
        if (current_node.is_left_son()) {
            current_node.parent.rotate_left();
        } else {
            current_node.parent.rotate_right();
        }
    }
    return repair_after_delete_cases_3_4(current_node);
}

function repair_after_delete_cases_3_4(current_node) {
    if (current_node.parent.node_color == NodeColor.Black &&
        current_node.brother().node_color == NodeColor.Black &&
        current_node.brother().left_son.node_color == NodeColor.Black &&
        current_node.brother().right_son.node_color == NodeColor.Black) {
        
        current_node.brother().node_color = NodeColor.Red;
        return repair_after_delete_case_1(current_node.parent);

    } else if (current_node.parent.node_color == NodeColor.Red &&
        current_node.brother().node_color == NodeColor.Black &&
        current_node.brother().left_son.node_color == NodeColor.Black &&
        current_node.brother().right_son.node_color == NodeColor.Black) {

        current_node.parent.node_color = NodeColor.Black;
        current_node.brother().node_color = NodeColor.Red;
        return recount_to_up(current_node.parent);
    }
    return repair_after_delete_case_5(current_node);
}

function repair_after_delete_case_5(current_node) {
    // current_node.brother().node_color == NodeColor.Black always true

    push_sub_hull_down(current_node.brother());

    if (current_node.brother().is_left_son() &&
        current_node.brother().left_son.node_color == NodeColor.Black) {

        current_node.brother().node_color = NodeColor.Red;
        current_node.brother().right_son.node_color = NodeColor.Black;

        push_sub_hull_down(current_node.brother().right_son);
        current_node.brother().rotate_left();

        recount(current_node.brother().left_son);

    } else if (current_node.brother().is_right_son() &&
        current_node.brother().right_son.node_color == NodeColor.Black) {
        
        current_node.brother().node_color = NodeColor.Red;
        current_node.brother().left_son.node_color = NodeColor.Black;

        push_sub_hull_down(current_node.brother().left_son);
        current_node.brother().rotate_right();

        recount(current_node.brother().right_son);
    }
    return repair_after_delete_case_6(current_node);
}

function repair_after_delete_case_6(current_node) {
    current_node.brother().node_color = current_node.parent.node_color;
    current_node.parent.node_color = NodeColor.Black;

    if (current_node.is_left_son()) {
        current_node.brother().right_son.node_color = NodeColor.Black;
        current_node.parent.rotate_left();
    } else {
        current_node.brother().left_son.node_color = NodeColor.Black;
        current_node.parent.rotate_right();
    }
    return recount_to_up(current_node.parent);
}