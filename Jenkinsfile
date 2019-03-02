library 'JenkinsBuilderLibrary'

helper.gitHubUsername = 'jakegough'
helper.gitHubRepository = 'jaytwo.NuGetCheck'
helper.gitHubTokenCredentialsId = 'github-personal-access-token-jakegough'
helper.nuGetCredentialsId = 'nuget-org-jaytwo'
helper.dockerRegistryCredentialsId = 'userpass-dockerhub-jakegough'
helper.cleanWsExcludePattern = 'out/testResults/**'

def registryImage = 'jakegough/jaytwo.nugetcheck'
def dockerTagPrefix = 'jaytwo.nugetcheck'

helper.run('linux && make && docker', {
    def timestamp = sh(returnStdout: true, script: "date +'%Y%m%d%H%M%S'").toString().trim()
    def dockerTag = "${dockerTagPrefix}-${timestamp}"
    def registryImageBeta = "${registryImage}:beta"
    def registryImagePrerelease = "${registryImageBeta}-${timestamp}"

    withEnv(["DOCKER_TAG=${dockerTag}"]) {
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
                    helper.pushNugetPackage('out/packed')
                }
                
                stage ('Publish Docker') {                        
                    sh "make docker"
                    if(env.BRANCH_NAME == 'master'){
                        helper.pushDockerImage(dockerTag, registryImage)
                    } else {
                        helper.tagDockerImage(dockerTag, registryImagePrerelease)
                        helper.pushDockerImage(dockerTag, registryImagePrerelease)
                        
                        helper.tagDockerImage(dockerTag, registryImageBeta)
                        helper.pushDockerImage(dockerTag, registryImageBeta)
                    }                        
                }
            }
        }
        finally {
            // not wrapped in a stage because it throws off stage history when cleanup happens because of a failed stage
            sh "make docker-cleanup"
            xunit tools: [MSTest(pattern: 'out/testResults/**/*.trx')]
        }
    }
})