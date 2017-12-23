'use strict';

$(document).ready(function () { 

    function draw(list, all_points) {
        let canvas = document.getElementById("canvas");
        let ctx = canvas.getContext("2d");
        //ctx.fillStyle = '#222222';
        ctx.clearRect(0, 0, 1000, 1000);

        ctx.fillStyle = '#FFC373';
        all_points.forEach(point => {
            ctx.beginPath();
            ctx.arc(point.x, -point.y, 7, 0, 2*Math.PI);
            ctx.fill();
        });

        ctx.strokeStyle = '#4A2A82';
        ctx.lineWidth = 2;
        ctx.beginPath();
        ctx.moveTo(list[list.length - 1].x, -list[list.length - 1].y);
        list.forEach(point => {
            ctx.lineTo(point.x, -point.y);
            ctx.moveTo(point.x, -point.y);
        });
        ctx.stroke();

        ctx.fillStyle = '#4A2A82';
        if (list.length >= 3) {
            for (let i = 0; i < list.length; ++i) {
                let pi = i == 0 ? list.length - 1 : i - 1;
                let ni = (i == list.length - 1) ? 0 : i + 1;

                if (determine_position(list[pi], list[ni], list[i]) != PointPosition.On || 
                    ((list[i].compare_to(list[pi]) > 0 && list[i].compare_to(list[ni]) > 0) || 
                        (list[i].compare_to(list[pi]) < 0 && list[i].compare_to(list[ni]) < 0))) {

                    ctx.beginPath();
                    ctx.arc(list[i].x, -list[i].y, 7, 0, 2*Math.PI);
                    ctx.fill();
                }
            }
        } else {
            list.forEach(point => {
                ctx.beginPath();
                ctx.arc(point.x, -point.y, 7, 0, 2*Math.PI);
                ctx.fill();
            });
        }
    }

    function process_point(to_add) {
        let found = false;
        
        all_points.forEach(point => {
            if (Math.sqrt((point.x - to_add.x) * (point.x - to_add.x) + (point.y - to_add.y) * (point.y - to_add.y)) <= 14) {                
                found = true;
            }
            if (Math.sqrt((point.x - to_add.x) * (point.x - to_add.x) + (point.y - to_add.y) * (point.y - to_add.y)) <= 10) {                

                hull.delete_point(point);

                all_points.splice(all_points.indexOf(point), 1);                
            }
        });

        if (!found) {
            hull.insert_point(to_add);
            all_points.push(to_add);
        }

        draw(hull.get_hull_as_array(), all_points);
    }

    function process_list(list_to_add) {
        list_to_add.forEach(point => {
            process_point(point);
        });
    }

    let hull = new DynamicConvexHull();
    let all_points = [];

    $("#canvas").on("click", function(e) {
        process_point(new Point(e.pageX, -e.pageY));
    });

    $("#random-delete").on("click", function(e) {
        let b = get_random_in_scope(0, all_points.length - 1);
        let f = get_random_in_scope(0, all_points.length - 1);
        if (b != f) {
            process_list(
                all_points.slice(Math.min(b, f), Math.max(b, f))
            );
        } else {
            process_list(
                all_points
            );
        }
    });
    $("#random-add").on("click", function(e) {
        process_list([
            new Point(get_random_in_scope(10, 480), get_random_in_scope(-10, -480)),
            new Point(get_random_in_scope(10, 480), get_random_in_scope(-10, -480)),
            new Point(get_random_in_scope(10, 480), get_random_in_scope(-10, -480)),
            new Point(get_random_in_scope(10, 480), get_random_in_scope(-10, -480)),
            new Point(get_random_in_scope(10, 480), get_random_in_scope(-10, -480)),
            new Point(get_random_in_scope(10, 480), get_random_in_scope(-10, -480)),
            new Point(get_random_in_scope(10, 480), get_random_in_scope(-10, -480)),
            new Point(get_random_in_scope(10, 480), get_random_in_scope(-10, -480)),
            new Point(get_random_in_scope(10, 480), get_random_in_scope(-10, -480)),
            new Point(get_random_in_scope(10, 480), get_random_in_scope(-10, -480)),
            new Point(get_random_in_scope(10, 480), get_random_in_scope(-10, -480)),
            new Point(get_random_in_scope(10, 480), get_random_in_scope(-10, -480)),
            new Point(get_random_in_scope(10, 480), get_random_in_scope(-10, -480)),
            new Point(get_random_in_scope(10, 480), get_random_in_scope(-10, -480)),
            new Point(get_random_in_scope(10, 480), get_random_in_scope(-10, -480)),
            new Point(get_random_in_scope(10, 480), get_random_in_scope(-10, -480))
        ]);
    });

    $("#tricky1").on("click", function(e) {
        process_list([
            new Point(10, -10),
            new Point(490, -10),
            new Point(10, -490),
            new Point(490, -490)
        ]);
    });
    $("#tricky2").on("click", function(e) {
        process_list([
            new Point(150, -200),
            new Point(170, -200),
            new Point(190, -200),
            new Point(210, -200),
            new Point(230, -200),
            new Point(250, -200),
            new Point(270, -200),
            new Point(290, -200),
            new Point(310, -200),
            new Point(330, -200)
        ]);
    });
    $("#tricky3").on("click", function(e) {
        process_list([
            new Point(100, -200),
            new Point(100, -220),
            new Point(100, -240),
            new Point(100, -260),
            new Point(100, -280),
            new Point(100, -300),
            new Point(100, -320),
            new Point(100, -340),
            new Point(100, -360),
            new Point(100, -380)
        ]);
    });
    $("#tricky4").on("click", function(e) {
        process_list([
            new Point(400, -220),
            new Point(380, -240),
            new Point(360, -260),
            new Point(340, -280),
            new Point(320, -300),
            new Point(300, -320),
            new Point(280, -340),
            new Point(260, -360),
            new Point(240, -380),
            new Point(220, -400)
        ]);
    });
    $("#tricky5").on("click", function(e) {
        process_list([
            new Point(460, -300),
            new Point(480, -300),
            new Point(460, -275),
            new Point(480, -275),
            new Point(460, -250),
            new Point(480, -250),
            new Point(460, -225),
            new Point(480, -225),
            new Point(460, -200),
            new Point(480, -200),
            new Point(460, -175),
            new Point(480, -175),
            new Point(460, -150),
            new Point(480, -150),
            new Point(460, -125),
            new Point(480, -125),
            new Point(460, -100),
            new Point(480, -100)
        ]);
    });

 });


