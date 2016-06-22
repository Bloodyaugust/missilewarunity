var fs = require('fs');
var path = require('path');

module.exports = function(grunt) {

  // Project configuration.
  grunt.initConfig({
    pkg: grunt.file.readJSON('package.json'),
    copy: {
      main: {
        cwd: 'public',
        dest: '../BrowserAssets/',
        expand: true,
        src: '**'
      }
    },
    express: {
      options: {
        script: 'index.js'
      },
      dev: {
        options: {
          args: []
        }
      }
    },
    watch: {
      html: {
        files: ['public/**/*'],
        tasks: ['copy'],
        options: {
          livereload: 12345
        }
      }
    }
  });

  grunt.loadNpmTasks('grunt-express-server');
  grunt.loadNpmTasks('grunt-contrib-copy');
  grunt.loadNpmTasks('grunt-contrib-watch');

  grunt.registerTask('no-default', function () {
    console.log('Default tasks are for the bad kind of lazy programmer. For shame!')
  });

  grunt.registerTask('default', ['no-default']);
  grunt.registerTask('dev', ['express:dev', 'watch']);
};
