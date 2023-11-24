const gulp = require('gulp');
const zip = require('gulp-zip');
const del = require('del');
const mkdirp = require('mkdirp');
const path = require('path');
const { exec } = require('child_process');
const os = require('os');
const {clean, restore, build, test, pack, publish, run} = require('gulp-dotnet-cli');
const version = `0.0.` + (process.env.BUILD_NUMBER || '0' + '-prerelease');
const configuration = process.env.BUILD_CONFIGURATION || 'Debug';
const targetProject = 'src/Student.Api.Template/Student.Api.Template.csproj';
//const runProject = 'src/Student.Api.Template.WebApi/Student.Api.Template.WebApi.csproj';
const cdkProject = 'src/Student.Api.Template.Infrastructure';
const migrationProject = 'src/Student.Api.Template.Migration/Student.Api.Template.Migration.csproj';

function getRuntime() {
    const rt = os.platform();
    if(rt === "win32") return "win-x64";
    if(rt === "darwin") return "osx-x64";
    return "linux-x64";
}

function _clean() {
    return gulp.src('*.sln', {read: false})
        .pipe(clean());
}

function _restore () {
    return gulp.src('*.sln', {read: false})
        .pipe(restore());
}

function _build() {
    return gulp.src('*.sln', {read: false})
        .pipe(build({configuration: configuration, version: version}));
}

function _test() {
    return gulp.src('**/*Tests.csproj', {read: false})
        .pipe(test({logger: `junit;LogFileName=${__dirname}/TestResults/xunit/TestOutput.xml`}))
}

function _distDir() {
    return new Promise((resolve, error) => {
        del(['dist'], {force: true}).then(
            () => { mkdirp('dist', resolve);
            });
    });
}

function _migrationsDir() {
    return new Promise((resolve, error) => {
        del(['migrations'], {force: true}).then(
            () => { mkdirp('migrations', resolve);
            });
    });
}

function _publish() {
    return gulp.src(targetProject, {read: false})
        .pipe(publish({
            configuration: configuration,
			version: version,
            output: path.join(process.cwd(), 'dist'),
            selfContained: true,
            runtime: getRuntime()
        }));
}

function _publish_local() {
    return gulp.src(targetProject, {read: false})
        .pipe(publish({
            configuration: configuration,
			version: version,
            output: path.join(process.cwd(), 'dist')
        }));
}

function _publishMigration() {
    return gulp.src(migrationProject, {read: false})
        .pipe(publish({
            configuration: configuration, version: version,
            output: path.join(process.cwd(), 'migrations')
        }));
}

// function _run() {
//     return gulp.src(runProject, {read: false})
//         .pipe(run());
// }

function _package() {
    return gulp.src('dist/**/*')
        .pipe(zip('LambdaPackage.zip'))
        .pipe(gulp.dest('dist'));
}

function _deploy() {
    return exec('npm run deploy', {cwd: cdkProject},(error, stdout, stderr) => {
        if (error) {
            console.error(`exec error: ${error}`);
            return;
        }
        console.log(`stdout: ${stdout}`);

        if(stderr)
            console.error(`stderr: ${stderr}`);
    });
}


exports.Build = gulp.series(_clean, _restore, _build);
exports.Test = gulp.series(_clean, _restore, _build, _test);
exports.Default = gulp.series(_clean, _restore, _build, _test);
exports.Publish = gulp.series(_distDir, _clean, _build, _publish);
// exports.Run = gulp.series(_distDir, _clean, _build, _run);
exports.Package = gulp.series(_distDir, _clean, _build, _publish, _package);
exports.PackageLocal = gulp.series(_distDir, _clean, _build, _publish_local, _package);
exports.Deploy = gulp.series(_distDir, _clean, _build, _test, _publish, _package, _deploy);
exports.PublishMigrations = gulp.series(_migrationsDir, _clean, _build, _publishMigration);

exports.CITest = gulp.series(_test);
exports.CIPackage = gulp.series(_distDir, _publish, _package);