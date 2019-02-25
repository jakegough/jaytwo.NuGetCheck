library 'JenkinsBuilderLibrary'

helper.gitHubUsername = 'jakegough'
helper.gitHubRepository = 'jaytwo.NuGetCheck'
helper.gitHubTokenCredentialId = 'github-personal-access-token-jakegough'

def dockerhub_username = 'jakegough'
def jenkins_credential_id_dockerhub = 'userpass-dockerhub-jakegough'

node('linux && make && docker') {
    try {
        stage('Clone') {
            cleanWs()
            checkout scm
        }
        stage('Set In Progress') {
            helper.updateGitHubBuildStatusInProgress();
        }
        
        def timestamp = sh(returnStdout: true, script: "date +'%Y%m%d%H%M%S'").toString().trim()        
        withEnv(["DOCKER_TAG_SUFFIX=-${timestamp}"]) {
            try {
                stage ('Build') {
                    sh "make docker-build"
                }
                stage ('Unit Test') {
                    sh "make docker-unit-test"
                }
                stage ('Integration Test') {
                    sh "make docker-integration-test"
                }
                stage ('Pack') {
                    if(env.BRANCH_NAME == 'master'){
                        sh "make docker-pack"
                    } else {
                        sh "make docker-pack-beta"
                    }
                }
                if(env.BRANCH_NAME == 'master' || env.BRANCH_NAME == 'develop'){
                    stage ('Publish') {
                        sh "make docker-publish"
                    }
                }
            }
            finally {
                // not wrapped in a stage because it throws off stage history when cleanup happens because of a failed stage
                sh "make docker-cleanup"
                xunit tools: [MSTest(pattern: 'out/testResults/**/*.trx')]
            }
        }
        stage('Set Success') {
            helper.updateGitHubBuildStatusSuccessful();
        }
    }
    catch(Exception e) {
        helper.updateGitHubBuildStatusFailed();
        throw e
    }
    finally {
        // not wrapped in a stage because it throws off stage history when cleanup happens because of a failed stage
        // clean workspace
        cleanWs(deleteDirs: true, patterns: [[type: 'EXCLUDE', pattern: 'out/testResults/**']])
    }
}
