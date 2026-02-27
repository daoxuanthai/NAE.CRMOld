/* [ ---- Gebo Admin Panel - datatables ---- ] */

	$(document).ready(function() {
		//* basic
		gebo_datatbles.dt_a();
		// horizontal scroll
		//gebo_datatbles.dt_b();
		////* large table
		//gebo_datatbles.dt_c();
		////* hideable columns
		//gebo_datatbles.dt_d();
		////* server side proccessing with hiden row
		//gebo_datatbles.dt_e();
	});
	
	//* calendar
	gebo_datatbles = {
		dt_a: function() {
			$('#dt_a').dataTable({
                "sDom": "<'row'<'span6'<'dt_actions'>l><'span6'f>r>t<'row'<'span6'i><'span6'p>>",
                "sPaginationType": "bootstrap_alt",
                "oLanguage": {
                    "sLengthMenu": "_MENU_ records per page"
                }
            });
		}
	};