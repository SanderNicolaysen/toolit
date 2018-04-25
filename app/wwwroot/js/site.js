// Vue instance that handles the notification area
var nmv = new Vue({
    el: '#notifications-container',
    data: {
        notifications: []
    },
    created: function () {
        var self = this;
        axios.get('/api/Notifications/unread')
        .then(function (response) {
            self.notifications = response.data;
        }); 
    },
    methods: {
        clickHandler: function(notification, event) {
            // Send a request to mark the notification as read
            axios.post('/api/Notifications', notification, { headers: { 'Content-Type': 'application/json' }});
        }
    }
})